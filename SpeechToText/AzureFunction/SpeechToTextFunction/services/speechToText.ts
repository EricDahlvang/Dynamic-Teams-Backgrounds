import { Context } from '@azure/functions';
import fetch from 'node-fetch';

const recognitionUrl = 'https://westus2.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?language=en-US&format=detailed';
const apiKey = process.env.SPEECH_KEY;

export interface SpeechToTextResponse {
    DisplayText: string;
}

export const getTextFromSpeech = async (buffer: Buffer): Promise<string> => {
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

    const json: SpeechToTextResponse = await response.json();
    
    return json.DisplayText;
}