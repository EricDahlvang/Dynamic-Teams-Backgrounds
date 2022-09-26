import { AzureFunction, Context, HttpRequest } from '@azure/functions'
import { getTextFromSpeech } from './services/getTextFromSpeech';
import { getPromptFromText } from './services/getPromptFromText';
import { getImageFromPrompt } from './services/getImageFromPrompt';

type Request = HttpRequest & {
    body: {
        SPEECH_KEY: string;
        conversationId: string;
        timeStamp: string;
        speech: string
    } | {
        SPEECH_KEY: string;
        conversationId: string;
        timeStamp: string;
        text: string
    }
}

type ContextResponse = Context & {
    res: {
        body: {
            text: string;
            prompt: string;
            conversationId: string;
            timeStamp: string;
            image: string;
        } | string
    }
}

enum Stage {
    Triggered = "TRIGGERED",
    HaveSpeech = "HAVE_SPEECH",
    HavePrompt = "HAVE_PROMPT",
    HaveImage = "HAVE_IMAGE"
}

const meetsCutoff = (context: ContextResponse, inTime: string, stage: Stage): boolean => {
    const inTimeEpoch = new Date(inTime).getTime();
    
    let cutoffSeconds;
    switch(stage) {
        case Stage.Triggered:
            cutoffSeconds = 5;
            break;
        case Stage.HaveSpeech:
            cutoffSeconds = 8;
            break;
        case Stage.HavePrompt:
            cutoffSeconds = 15;
            break;
        case Stage.HaveImage:
            cutoffSeconds = 20;
            break;
        default:
            cutoffSeconds = 25;
    }

    const cutoffEpoch = inTimeEpoch + (cutoffSeconds * 1000);

    const good = Date.now() <= cutoffEpoch;
    if (!good) {
        context.res = {
                status: 408,
                body: `Cutoff not met: ${stage}`
            }
        context.log(`ERROR: Cutoff not met: ${stage}`);
    }

    return good;
}

let unresolvedImageRequests = 0;

const imageQueueNotTooLong = (context: ContextResponse): boolean => {
    if (unresolvedImageRequests > 2) {
        const body = `Error: Image request queue is too long: ${unresolvedImageRequests}`;
        context.log(body);
        context.res = {
            status: 429,
            body,
        };
        return false;
    }

    return true;
}

const okayToContinue = (context: ContextResponse, timeStamp: string, stage: Stage): boolean => {
    return meetsCutoff(context, timeStamp, stage) && imageQueueNotTooLong(context);
}

const httpTrigger: AzureFunction = async function (context: ContextResponse, req: Request): Promise<void> {
    if (req.body.SPEECH_KEY !== process.env.SPEECH_KEY) {
        context.res = {
            status: 403,
            body: 'Unauthorized. Use the `SPEECH_KEY` and pass it into the request body under the SPEECH_KEY property'
        };
        return;
    }

    context.log('HTTP trigger function processed a request.');

    if (!okayToContinue(context, req.body.timeStamp, Stage.Triggered)) return;

    let text = req.body.text;
    if (!text) {
        let buffer: Buffer;
        try {
            buffer = Buffer.from(req.body.speech, 'binary');
            context.log('Converted speech WAV to buffer, successfully');
        } catch(err) {
            context.res = {
                status: 500,
                body: `Error converting speech WAV into buffer\n${JSON.stringify(err)}`
            }
            return;
        }

        try {
            text = await getTextFromSpeech(buffer);
            context.log(`Got text from speech: ${text}`);
        } catch(err) {
            context.res = {
                status: 500,
                body: `Error converting Speech to Text\n${JSON.stringify(err)}`
            };
            return;
        }
    }

    if (!okayToContinue(context, req.body.timeStamp, Stage.HaveSpeech)) return;  
    
    let prompt: string;
    try {
        prompt = await getPromptFromText(text);
        context.log(`Got prompt from text: ${prompt}`);
    } catch(err) {
        context.res = {
            status: 500,
            body: `Error getting prompt from text\n${JSON.stringify(err)}`
        };
        return;
    }

    if (!okayToContinue(context, req.body.timeStamp, Stage.HavePrompt)) return;

    let image: string;
    try {
        unresolvedImageRequests++;
        image = await getImageFromPrompt(prompt, req.body.conversationId, req.body.timeStamp);
        unresolvedImageRequests--;
        context.log(`Got image from prompt: ${prompt}`);
    } catch(err) {
        unresolvedImageRequests--;
        context.res = {
            status: 500,
            body: `Error getting image from prompt\n${JSON.stringify(err)}`
        };
        return;
    }    

    if (!meetsCutoff(context, req.body.timeStamp, Stage.HaveImage)) return;

    context.res = {
        body: {
            text,
            prompt,
            conversationId: req.body.conversationId,
            timeStamp: req.body.timeStamp,
            image
        }
    };

};

export default httpTrigger;