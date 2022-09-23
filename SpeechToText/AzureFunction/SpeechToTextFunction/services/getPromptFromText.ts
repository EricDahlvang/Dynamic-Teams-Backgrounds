import fetch from 'node-fetch';

interface PromptResponse {
    Entities: string[];
    Negative: number;
    Neutral: number;
    Positive: number;
    Prompt: string;
    Text: string;
}

interface PromptRequestBody {
    Text: string;
    MinConfidenceScore: number;
    PromptContentType: 'Entities' | 'RawText'
}

export const getPromptFromText = async (text: string): Promise<string> => {
    const body: PromptRequestBody = {
        Text: text,
        MinConfidenceScore: 0.1,
        PromptContentType: 'RawText'
    };

    const response = await fetch(process.env.PROMPT_URL, {
        method: 'POST',
        body: JSON.stringify(body),
        headers: {
            'Content-Type': 'application/json'
        }
    });

    const json: PromptResponse = await response.json();
    
    return json.Prompt;
}