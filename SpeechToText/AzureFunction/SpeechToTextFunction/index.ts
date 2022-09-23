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

    let image: string;
    try {
        context.log(`Got image from prompt: ${prompt}`);
        image = await getImageFromPrompt(prompt, req.body.conversationId, req.body.timeStamp);
        context.log(`Got image from prompt: ${prompt}`);
    } catch(err) {
        context.res = {
            status: 500,
            body: `Error getting image from prompt\n${JSON.stringify(err)}`
        };
        return;
    }

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