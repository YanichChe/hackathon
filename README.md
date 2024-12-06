### Задача 5: Web службы 

Теперь у нас 5 джунов и 5 тимлидов (файлы Juniors5.csv и Teamleads5.csv). Формула расчёта индекса удовлетворённости меняется соответственно (5 за наиболее предпочтительного напарника 4 за следующего и т.д.) 

Списки тут: https://github.com/georgiy-sidorov/CSHARP_2024_NSU/tree/main

Реализовать в виде отдельных экземпляров приложения в Docker контейнерах: 

    Каждого тимлида; 

    Каждого джуна; 

    HR-менеджера; 

    HR-директора. 


Разработчики отправляют свои запросы по HTTP, HR-менеджеру, который определяет распределение. 

HR-менеджер отправляет HR-директору по HTTP все предпочтения всех участников, и итоговое распределение. 

HR-директор считает гармоничность, выводит её в консоль и сохраняет в БД (как в предыдущей задаче). 

Хакатон проводится 1 раз. 

Оформить всё едином docker-compose.yml файлом и запускать одной командой.  

Пример файла docker-compose.yaml  

services: 

  teamlead-1: 

    image: developer:latest 

    deploy: 

      replicas: 1 

    args: 

      - type=teamlead 

      - id=1 

  teamlead-2: 

    image: developer:latest 

    deploy: 

      replicas: 1 

    args: 

      - type=teamlead 

      - id=1

… 

![image](https://github.com/user-attachments/assets/76d3dadb-20f4-4407-85cc-25bb833835af)
