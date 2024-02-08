# Парсер сайта для друга
Программа загружает медиаконтент с [сайта](https://www.rong-chang.com/).
<br/>Медиаконтент состоит из заданий по английскому языку, которые имеют название, текст и аудио.
<br/>Данные сериализуются и сохраняются в `serialized.json`.
<br/>Аудиофайлы скачиваются в `audio\` с уникальными именами guid.
<br/>Сериализованные данные выглядят следующим образом:
```json
{
  "Name": "English for Children (I)",
      "Exersises": [
        {
          "Name": " 1. Red Ball on the Floor ",
          "Audio": "2b01ef85-8453-44a1-8f39-52d11ed554de",
          "Text": "The ball is on the floor. It is a red ball. It is a rubber ball. The baby looks at the ball. The cat looks at the ball. The cat is black. The cat walks over to the ball. The cat hits the ball with its paw. The ball rolls on the floor. The baby smiles.",
          "audioUrl": "https://www.rong-chang.com/easykids/audio/ekid001.mp3",
          "exerciseUrl": "https://www.rong-chang.com/easykids/ekid/easykid001.htm"
        },
      ]
}
```
