## Все на хакатон!
![image](https://github.com/user-attachments/assets/3c79e9cb-cde7-4158-a47b-e166c8052f89)

В одной IT компании захотели сформировать команды мечты (DreamTeam). 
Для этого начинающий HR менеджер (HRManager) предложил умудрённому опытом HR директору (HRDirector) провести хакатон (Hackathon).

На хакатон позвали 20 джунов (Junior) и 20 тимлидов (TeamLead). Разработчикам предложили поработать вместе и к концу 
мероприятия составить списки коллег (Wishlist), с которыми они хотели бы работать в одной команде. Каждый джун составляет 
список из 20 тимлидов, ставя на первое место наиболее предпочтительного кандидата, на второе следующего по предпочтительности 
и далее по убывающей. Тимлиды, в свою очередь, таким же образом оценивают джунов. Все списки должны состоять из 20 пунктов, пункты не должны повторяться. В списках тимлидов могут быть только джуны, в списках джунов только тимлиды.

Все эти списки передаются нашему начинающему HR менеджеру, который уже пообещал придумать инновационную стратегию формирования команд мечты. 
Каждая команда должна состоять из пары: Тимлид + Джун. Список команд он передаст HR директору.

Все было бы просто, но умудрённый опытом HR директор придумал как математически точно оценить 
гармоничность команд. Для этого он предложил посчитать индекс удовлетворённости каждого из разработчиков. Например, 
если тимлиду попался наиболее желаемый джун, то он получает 20 очков удовлетворённости, если второй по списку, то 19 и 
т.д. вплоть до 1. Точно так же рассчитывается индекс удовлетворённости джунов их тимлидами.

Далее HR директор подсчитывает среднее гармоническое индексов удовлетворённости всех участников. Именно это число 
HR директор предложил считать гармоничностью распределения. Основная цель начинающего HR менеджера при выборе стратегии 
распределение по командам – чтобы гармоничность мероприятия была как можно выше.

Хакатон проводится несколько раз. Необходимо вычислить среднюю гармоничность как среднее арифметическое по всем 
мероприятиям, т.е.  вычислить среднее арифметическое средних гармонических. 