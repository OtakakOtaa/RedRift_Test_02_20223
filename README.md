# RedRift_Test_02_20223

Асинхронные задачи были реализованны через карутины C#, хотя сам предпочитаю работать с Task, решение было принято всилу уменьшение импорта.
Также каждая карта имеет собственный канвас, что не сильно производительно, но имееет простоту в решении.
Также решение одной системы для анимирования множество карт, окозалось более плохим, чем один к одному, так-как пришлось думать о атамарности кеш-данных анимации.  
