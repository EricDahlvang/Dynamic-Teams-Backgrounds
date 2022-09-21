import { AzureFunction, Context, HttpRequest } from "@azure/functions"
import { getTextFromSpeech } from './services/speechToText';

type Request = HttpRequest & {
    query: {
        SPEECH_KEY: string;
        conversationId: string;
        timestamp: string;
    }
}

type ContextResponse = Context & {
    res: {
        body: {
            prompt: string;
            conversationId: string;
            timestamp: string;
        } | string
    }
}

const httpTrigger: AzureFunction = async function (context: ContextResponse, req: Request): Promise<void> {
    if (req.query.SPEECH_KEY !== process.env.SPEECH_KEY) {
        context.res = {
            status: 403,
            body: 'Unauthorized. Use the `SPEECH_KEY` and pass it into the request body under the SPEECH_KEY property'
        };
        return;
    }

    context.log('HTTP trigger function processed a request.');

    let buffer: Buffer;
    try {
        buffer = Buffer.from(req.body, 'binary');
        context.log('Converted speech WAV to buffer, successfully');
    } catch(err) {
        context.res = {
            status: 500,
            body: `Error converting speech WAV into buffer\n${JSON.stringify(err)}`
        }
        return;
    }

    let prompt: string;
    try {
        prompt = await getTextFromSpeech(buffer, context);
    } catch(err) {
        context.res = {
            status: 500,
            body: `Error converting Speech to Text\n${JSON.stringify(err)}`
        };
        return;
    }

    // TODO: Add prompt beautification

    // TODO: Send this off to Stable Diffusion

    context.res = {
        // status: 200, /* Defaults to 200 */
        body: {
            prompt: prompt,
            conversationId: req.query.conversationId,
            timestamp: req.query.timestamp
        }
    };

};

export default httpTrigger;