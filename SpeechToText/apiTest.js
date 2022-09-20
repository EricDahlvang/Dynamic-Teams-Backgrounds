const path = require('path');
const fs = require('fs');
const fetch = require('node-fetch');

require('dotenv').config({
  path: path.resolve(__dirname, '.env')
});

const apiKey = process.env.SPEECH_KEY;

const recognitionUrl = 'https://westus2.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?language=en-US&format=detailed';

const speech = fs.readFileSync(path.resolve(__dirname, 'this-is-a-sample-recording-to-test-speech-to-text-transcription.wav'));

fetch(recognitionUrl, {
    method: 'POST',
    body: speech,
    headers: {
        Accept: 'application/json;text/xml',
        'Content-Type': 'audio/wav',
        'Ocp-Apim-Subscription-Key': apiKey,
        'Transfer-Encoding': 'chunked',
        Expect: '100-continue'
    }
}).then(response => {
    response.json().then(result => {
        console.log(JSON.stringify(result, null, 2))
    })
});

