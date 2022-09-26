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

const httpTrigger: AzureFunction = async function (context: ContextResponse, req: Request): Promise<void> {
    if (req.body.SPEECH_KEY !== process.env.SPEECH_KEY) {
        context.res = {
            status: 403,
            body: 'Unauthorized. Use the `SPEECH_KEY` and pass it into the request body under the SPEECH_KEY property'
        };
        return;
    }

    context.log('HTTP trigger function processed a request.');

    // TODO: Queue or cutoff limits
    let text = req.body.text;
    if (!text) {
        let buffer: Buffer;
        try {
            buffer = Buffer.from(req.body.speech, 'binary');
        } catch(err) {
            context.res = {
                status: 500,
                body: `Error converting speech WAV into buffer\n${JSON.stringify(err)}`
            }
            return;
        }
        context.log('Converted speech WAV to buffer, successfully');

        try {
            text = await getTextFromSpeech(buffer);
        } catch(err) {
            context.res = {
                status: 500,
                body: `Error converting Speech to Text\n${JSON.stringify(err)}`
            };
            return;
        }
    }
    context.log(`Got text from speech: ${text}`);
    
    
    let prompt: string;
    try {
        prompt = await getPromptFromText(text);
    } catch(err) {
        context.res = {
            status: 500,
            body: `Error getting prompt from text\n${JSON.stringify(err)}`
        };
        return;
    }
    context.log(`Got prompt from text: ${prompt}`);

    let image: string;
    try {
        context.log(`Got image from prompt: ${prompt}`);
        image = await getImageFromPrompt(prompt, req.body.conversationId, req.body.timeStamp);
    } catch(err) {
        context.res = {
            status: 500,
            body: `Error getting image from prompt\n${JSON.stringify(err)}`
        };
        return;
    }
    context.log(`Got image from prompt: ${prompt}`);

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