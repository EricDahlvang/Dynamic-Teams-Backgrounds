const path = require('path');
const fs = require('fs');
const fetch = require('node-fetch');

require('dotenv').config({
  path: path.resolve(__dirname, '.env')
});

const localUrl = 'http://localhost:7071/api/SpeechToTextFunction';

const speech = fs.readFileSync(path.resolve(__dirname, 'this-is-a-sample-recording-to-test-speech-to-text-transcription.wav'), { encoding: 'binary'});

const queryParams = new URLSearchParams({
    conversationId: 'some:uniqueConversation',
    timestamp: new Date().getTime(),
    SPEECH_KEY: process.env.SPEECH_KEY
})

fetch(localUrl + '/?' + queryParams, {
    method: 'POST',
    body: speech,
    headers: {
        Accept: '*/*',
        'Content-Type': 'audio/wav',
        'Transfer-Encoding': 'chunked'
    }
})
    .then(response => {
        response.json()
            .then(result => {
                console.log(JSON.stringify(result, null, 2));
            })
            .catch(err => {
                console.error(err);
            })
    .catch(err => {
        console.error(err);
    })
})