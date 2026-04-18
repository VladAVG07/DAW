using Lab06.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Lab06.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        await context.Database.MigrateAsync();

        string[] roleNames = ["Admin", "User"];
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var adminEmail = "admin@newsportal.com";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                FullName = "Administrator",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }

        if (await context.Categories.AnyAsync() || await context.Articles.AnyAsync())
        {
            return;
        }

        var technology = new Category { Name = "Tehnologie" };
        var sport = new Category { Name = "Sport" };
        var culture = new Category { Name = "Cultura" };
        var actualitate = new Category { Name = "Actualitate" };

        await context.Categories.AddRangeAsync(technology, sport, culture, actualitate);
        await context.SaveChangesAsync();

        await context.Articles.AddRangeAsync(
            new Article
            {
                Title = "Universitatile testeaza platforme AI pentru predare si evaluare",
                Content = "Mai multe universitati europene analizeaza modul in care instrumentele bazate pe inteligenta artificiala pot sprijini activitatea didactica. Printre scenariile discutate se numara generarea de exercitii, feedback automat pentru teme si asistenta in organizarea materialelor de curs. Cadrele didactice atrag insa atentia ca astfel de solutii trebuie folosite cu prudenta, mai ales in evaluare.",
                PublishedAt = new DateTime(2026, 3, 10),
                CategoryId = technology.Id
            },
            new Article
            {
                Title = "Noi generatii de procesoare promit eficienta energetica mai buna",
                Content = "Producatorii de hardware au prezentat in ultimele luni noi arhitecturi de procesoare orientate atat spre performanta, cat si spre reducerea consumului de energie. Accentul este pus pe laptopuri mai silentioase, autonomie extinsa si sarcini asistate de unitati dedicate pentru AI. Analistii spun ca directia pietei este clara: mai multa performanta, cu accent pe costuri energetice mai mici.",
                PublishedAt = new DateTime(2026, 3, 12),
                CategoryId = technology.Id
            },
            new Article
            {
                Title = "Companiile investesc in centre de date optimizate pentru sarcini AI",
                Content = "Interesul crescut pentru modele de inteligenta artificiala a determinat companiile sa isi regandeasca infrastructura. Tot mai multe investitii sunt directionate catre centre de date optimizate pentru acceleratoare hardware si procesare paralela. Specialistii subliniaza ca provocarile nu tin doar de putere de calcul, ci si de racire, consum energetic si costul operational pe termen lung.",
                PublishedAt = new DateTime(2026, 3, 16),
                CategoryId = technology.Id
            },
            new Article
            {
                Title = "Start de sezon in Formula 1, cu accent pe noile pachete tehnice",
                Content = "Echipele au prezentat noile monoposturi si au oferit primele indicii despre directia tehnica a sezonului. Atentia este concentrata pe eficienta aerodinamica, fiabilitate si adaptarea la circuitele din primele curse. Pilotii au declarat ca diferentele dintre echipe par mai mici decat in sezoanele trecute, ceea ce ar putea duce la un campionat mai echilibrat.",
                PublishedAt = new DateTime(2026, 3, 15),
                CategoryId = sport.Id
            },
            new Article
            {
                Title = "Turneu international de tenis aduce la start jucatori din topul mondial",
                Content = "Competitia reuneste sportivi cu experienta, dar si jucatori aflati in plina ascensiune. Organizatorii se asteapta la meciuri echilibrate si la un interes crescut din partea publicului, mai ales dupa rezultatele surprinzatoare din ultimele turnee. Antrenorii spun ca programul incarcat al sezonului va influenta ritmul de joc si strategia participantilor.",
                PublishedAt = new DateTime(2026, 3, 11),
                CategoryId = sport.Id
            },
            new Article
            {
                Title = "Cluburile europene isi pregatesc loturile pentru fazele decisive ale sezonului",
                Content = "In competitiile continentale, perioada urmatoare este considerata decisiva pentru obiectivele sportive si financiare ale cluburilor. Staff-urile tehnice pun accent pe rotatia jucatorilor, recuperare si gestionarea accidentarilor. Comentatorii sportivi remarca faptul ca diferenta dintre echipe este tot mai des facuta de organizarea defensiva si de consistenta lotului pe termen lung.",
                PublishedAt = new DateTime(2026, 3, 18),
                CategoryId = sport.Id
            },
            new Article
            {
                Title = "Festivalul de film european aduce proiectii speciale si dezbateri cu regizori",
                Content = "Editia din acest an include atat filme premiate recent, cat si productii independente prezentate pentru prima data publicului larg. Organizatorii au pregatit sesiuni de intrebari si raspunsuri, intalniri cu regizori si discutii despre transformarile industriei cinematografice. Publicul este invitat sa participe nu doar la proiectii, ci si la ateliere dedicate studentilor si tinerilor cineasti.",
                PublishedAt = new DateTime(2026, 3, 9),
                CategoryId = culture.Id
            },
            new Article
            {
                Title = "Muzeele extind programele educationale pentru publicul tanar",
                Content = "Tot mai multe institutii culturale dezvolta programe interactive pentru elevi si studenti, incercand sa apropie patrimoniul de noile generatii. Atelierele includ ghidaje tematice, activitati digitale si expozitii cu componente multimedia. Reprezentantii muzeelor spun ca interesul pentru astfel de initiative este in crestere, mai ales atunci cand continutul este prezentat intr-o forma accesibila si actuala.",
                PublishedAt = new DateTime(2026, 3, 14),
                CategoryId = culture.Id
            },
            new Article
            {
                Title = "Expozitie de arta contemporana exploreaza relatia dintre tehnologie si memorie",
                Content = "Noua expozitie reuneste lucrari multimedia, instalatii si proiecte video care discuta felul in care tehnologia influenteaza modul in care pastram si reinterpretam memoria colectiva. Curatorii au construit traseul astfel incat vizitatorii sa treaca prin mai multe forme de expresie artistica, de la fotografie si sunet pana la instalatii interactive. Evenimentul este insotit de dezbateri si tururi ghidate.",
                PublishedAt = new DateTime(2026, 3, 17),
                CategoryId = culture.Id
            },
            new Article
            {
                Title = "A murit Chuck Norris. Celebrul actor si artist martial avea 86 de ani",
                Content = "Lumea cinematografiei si a artelor martiale este in doliu dupa disparitia lui Chuck Norris, actorul american cunoscut pentru rolurile din seriale si filme de actiune care au marcat generatii intregi. Norris s-a nascut pe 10 martie 1940 in Ryan, Oklahoma, si a devenit un simbol al culturii pop mondiale. Cariera sa a inclus titluri de campion mondial la karate, apoi zeci de filme si serialul Walker, Texas Ranger, difuzat intre 1993 si 2001. Familia a confirmat decesul printr-un comunicat, multumind fanilor pentru mesajele de condoleante primite din toata lumea.",
                PublishedAt = new DateTime(2026, 3, 20),
                ImagePath = "/images/chuck-norris-in-the-expendables-2.png",
                CategoryId = actualitate.Id
            },
            new Article
            {
                Title = "Criza carburantilor. Economistii avertizeaza: pretul la pompa ar putea depasi 12 lei/litru",
                Content = "Preturile carburantilor au crescut semnificativ in ultima perioada, iar economistii spun ca scenariul in care benzina si motorina ajung la 12-13 lei pe litru nu este de domeniul fictiunii. Factorii principali invocati sunt tensiunile geopolitice din Orientul Mijlociu, reducerea productiei OPEC+ si deprecierea leului fata de dolar. Reprezentantii companiilor de transport avertizeaza ca o noua crestere va afecta direct preturile la raft, in special la produsele alimentare. Guvernul a anuntat ca analizeaza mecanisme de plafonare temporara, dar nu a oferit detalii concrete.",
                PublishedAt = new DateTime(2026, 3, 21),
                ImagePath = "/images/alimentare_pompa_benzina.jpg",
                CategoryId = actualitate.Id
            },
            new Article
            {
                Title = "Razboi in Orientul Mijlociu. Trump anunta o posibila retragere partiala a trupelor americane",
                Content = "Presedintele american Donald Trump a declarat ca Statele Unite analizeaza o retragere partiala a fortelor militare din Orientul Mijlociu, in contextul escaladarii conflictului dintre Israel si Iran. Iranul a lansat o serie de rachete balistice asupra teritoriului israelian, vizand inclusiv zona Dimona, unde se afla instalatii nucleare. Israelul a ripostat cu atacuri aeriene asupra pozitiilor militare iraniene din Siria si Liban. Comunitatea internationala a cerut o dezescaladare imediata, iar ONU a convocat o sesiune de urgenta a Consiliului de Securitate.",
                PublishedAt = new DateTime(2026, 3, 21),
                ImagePath = "/images/trump.png",
                CategoryId = actualitate.Id
            },
            new Article
            {
                Title = "Clasamentul reciclarii din Bucuresti si Ilfov. Localitatile cu cele mai bune rezultate",
                Content = "Autoritatea Nationala pentru Protectia Mediului a publicat datele oficiale privind rata de reciclare in Bucuresti si judetul Ilfov pentru anul 2025. Rezultatele arata diferente semnificative intre sectoare si localitati: sectorul 2 si orasul Voluntari se situeaza in fruntea clasamentului, cu rate de colectare selectiva de peste 35%, in timp ce alte zone raman sub media nationala. Specialistii atrag atentia ca infrastructura de colectare este insuficienta si ca educatia cetatenilor ramane principala bariera in calea imbunatatirii indicatorilor.",
                PublishedAt = new DateTime(2026, 3, 21),
                ImagePath = "/images/clasamentul_reciclarii.png",
                CategoryId = actualitate.Id
            }
        );

        await context.SaveChangesAsync();
    }
}
