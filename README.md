### Задача 2: .Net Generic Host

Поместить песочницу в .Net Generic Host. Выделить в отдельные зависимости:

* Класс, реализующий проведение одного хакатона;
* Класс, реализующий HRManager;
* Класс, реализующий HRDirector;
* Класс, реализующий HRManager;

Например, инициализация хоста может выглядеть следующим образом

        using Microsoft.Extensions.DependencyInjection;
        
        using Microsoft.Extensions.Hosting;
        
        
        
        var host = Host.CreateDefaultBuilder(args)

        .ConfigureServices((hostContext, services) => 

	    { 

            services.AddHostedService<HackathonWorker>(); 

            services.AddTransient<Hackathon>(_ => new Hackathon()); 

            services.AddTransient<ITeamBuildingStrategy, TeamLeadsHateTheirJuniorsTeamBuildingStrategy>(); 

            services.AddTransient<HrManager>();

            services.AddTransient<HrDirector>(); 

  	    }).Build(); 

        host.Run(); 