import { AzureFunction, Context, HttpRequest } from "@azure/functions"
import fetch from 'node-fetch';

interface Request extends HttpRequest {
    body: {
        speech: string,
        conversationId: string,
        uniqueId: string
    }
}

const httpTrigger: AzureFunction = async function (context: Context, req: Request): Promise<void> {
    context.log('HTTP trigger function processed a request.');

    const recognitionUrl = 'https://westus2.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?language=en-US&format=detailed';
    const apiKey = process.env.SPEECH_KEY;

    const buffer = Buffer.from(req.body as any, 'binary');

    const response = await fetch(recognitionUrl, {
        method: 'POST',
        body: buffer,
        headers: {
            Accept: 'application/json;text/xml',
            'Content-Type': 'audio/wav',
            'Ocp-Apim-Subscription-Key': apiKey,
            'Transfer-Encoding': 'chunked',
            Expect: '100-continue'
        }
    });

    const json = await response.json();

    // TODO: Send this off to Stable Diffusion

    context.res = {
        // status: 200, /* Defaults to 200 */
        body: json
    };

};

export default httpTrigger;