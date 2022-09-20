const path = require('path');
const fs = require('fs');
const fetch = require('node-fetch');

const localUrl = 'http://localhost:7071/api/SpeechToTextFunction';

const speech = fs.readFileSync(path.resolve(__dirname, 'this-is-a-sample-recording-to-test-speech-to-text-transcription.wav'), { encoding: 'binary'});

const formData = new FormData();
formData.append('speech', speech);
formData.append('conversationId', 'a:1234');
formData.append('speechId', 'somethingUnique')

fetch(localUrl, {
    method: 'POST',
    body: speech,
    headers: {
        Accept: 'application/json',
        // 'Content-Type': 'audio/wav',
        'Transfer-Encoding': 'chunked'
    }
}).then(response => {
    response.json().then(result => {
        console.log(JSON.stringify(result, null, 2));
    })
})