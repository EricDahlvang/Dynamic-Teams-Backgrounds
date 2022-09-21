import fetch from 'node-fetch';

interface ImageRequestBody {
    prompt: string;
    conversationId: string;
    timeStamp: string;
    authKey: string;
}

export const getImageFromPrompt = async (prompt: string, conversationId: string, timeStamp: string): Promise<any> => {
    const body: ImageRequestBody = {
        prompt,
        conversationId,
        timeStamp,
        authKey: process.env.SD_KEY
    };

    const response = await fetch(process.env.SD_URL, {
        method: 'POST',
        body: JSON.stringify(body),
        headers: {
            'Content-Type': 'application/json'
        }
    });

    const json = await response.json();
    
    return json.base64EncodedImage;
}