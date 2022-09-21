const path = require('path');
const fs = require('fs');
const fetch = require('node-fetch');

require('dotenv').config({
  path: path.resolve(__dirname, '.env')
});

const localUrl = 'http://localhost:7071/api/SpeechToTextFunction';

const speech = fs.readFileSync(path.resolve(__dirname, 'this-is-a-sample-recording-to-test-speech-to-text-transcription.wav'), { encoding: 'binary'});

fetch(localUrl, {
    method: 'POST',
    body: JSON.stringify({ 
        speech,
        conversationId: 'some:uniqueConversation',
        timeStamp: new Date().toISOString(),
        SPEECH_KEY: process.env.SPEECH_KEY
    }),
    headers: {
        Accept: '*/*',
        'Content-Type': 'application/json',
        'Transfer-Encoding': 'chunked'
    }
})
    .then(response => {
        response.json()
            .then(result => {
                console.log(JSON.stringify(result, null, 2));
                const base64Data = result.image.replace(/^data:image\/png;base64,/, "");
                fs.writeFile("generatedImage.png", base64Data, 'base64', (err) => {
                    console.log(err);
                });
            })
            .catch(err => {
                console.error(err);
            })
    .catch(err => {
        console.error(err);
    })
})