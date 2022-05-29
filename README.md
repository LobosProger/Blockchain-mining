# Blockchain-mining
Уникальный скрипт, реализующий работу майнинга прямо в Unity!

* Данный код реализует почти полноценную работу блокчейна с майнингом. Однако распространение информации и самой базы данных не предусмотрено. Код написан лишь для ознакомления работы майнинга, то как он устроен и работает. 
* Для работы этого скрипта просто создайте пустой игровой объект, добавьте к нему данный скрипт. Теперь в поле Data впишите любой текст - что угодно, а в поле Difficulty впишите символы из 16-ричной системы счисления маленькими буквами. Это требуется для того, чтобы задать правило намайненного блока. Можно вписать например просто нули - как в системе Биткоин (например, два нуля, т.е. в поле Difficulty будет выглядеть следующая запись: "00"). Теперь нажмите на кнопку Mine, чтобы сгенерировать намайненный блок. 
* Подождите немного и после этого в Blockchain Info появится новая запись, где состоит из: 
* Block (номер блока) 
* Nonce (случайного числа, требуемого для майнинга блока)
* Data (данные, которые Вы вписали в поле Data) 
* Previous Hash (В блокчейне все блоки соединены с помощью предыдущего хэша)
* Hash (подобранный хэш согласно правилу)
* Time Of Mining (время майнинга (указано в секундах))
* Hashrate In Seconds (хэшрейт вашего компьютера в секундах, то есть количество подбираемых хэшей в секунду)

* После майнинга в вкладке Console появится новая запись - эта та самая запись, которая помещается в алгоритм SHA256 для подбора нужного хэша по правилу. Вы можете скопировать данную строку данных и вставить в онлайн сервис по вычислению SHA256 (например, https://emn178.github.io/online-tools/sha256.html) для того, чтобы удостовериться в работе данного алгоритма майнинга (например, я выставил в поле Difficulty: 00, нажал на кнопку Mine и появилась следующая запись в Console "1The Lobos Robotics NFT. All rights reserved.01" (кавычки НЕ нужно копировать отсюда), вставив в поле Input получаем следующую запись: "00b93e033b278155946fc451304a2aec970fc9fc01401b4897f2cd913aec975f", т.е. я выставил правило майнинга, которое вычисляет хэш по двум нулям)
