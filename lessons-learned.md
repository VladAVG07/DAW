# Lessons learned - Lab 06

## De ce folosim Repository Pattern?
Folosim Repository Pattern ca sa separam logica de acces la date de restul aplicatiei.
Repository-ul ascunde detaliile EF Core (`Include`, `Where`, `OrderBy`, `ToListAsync`) si ofera metode clare pentru operatii pe entitati.
Astfel evitam duplicarea query-urilor in mai multe controllere si avem cod mai usor de mentinut.

## Ce s-ar intampla daca apelam _context direct din controller?
Daca apelam `_context` direct din controller:
- controllerul devine prea incarcat (UI + data access + reguli de business)
- aceleasi query-uri se repeta in mai multe actiuni
- testarea devine dificila, pentru ca trebuie simulat EF Core sau baza de date
- orice schimbare in modelul de date afecteaza direct controllerul

## De ce avem un Service Layer separat?
Service Layer exista pentru logica de business, adica reguli care nu tin strict de stocare in baza de date.
Controllerul ar trebui sa coordoneze request/response, iar serviciul sa aplice reguli si fluxuri.

## Ce logica ar ajunge in controller fara Service Layer?
Fara Service Layer, in controller ar ajunge:
- setari de business (ex: `PublishedAt = DateTime.Now`)
- mapari intre entitati si view models
- validari si verificari suplimentare
- operatii compuse (citire + update + save in mai multi pasi)

Rezultatul ar fi controllere mari, greu de citit si greu de testat.

## De ce folosim interfete (IArticleRepository, IArticleService)?
Interfetele decupleaza codul de implementare.
Controllerul depinde de un contract (`IArticleService`), nu de o clasa concreta.
Asta permite:
- inlocuirea usoara a implementarii
- mock-uri in teste unitare
- respectarea principiului Dependency Inversion

## Exemplu concret din cod
In `ArticlesController`, actiunea `Index` nu mai foloseste `_context.Articles...`, ci:

`var articles = await _articleService.GetPagedAsync(currentPage, pageSize, cancellationToken);`

Iar in `ArticleService`, datele sunt obtinute prin repository:

`var articles = await _unitOfWork.Articles.GetPagedWithDetailsAsync(safePage, safePageSize, cancellationToken);`

Deci controllerul vorbeste cu service-ul, iar service-ul cu repository-ul.

## Cum ajuta structura asta pentru un API REST?
Daca adaugam un API REST, controllerul API poate reutiliza aceleasi servicii (`IArticleService`, `ICategoryService`) fara sa rescriem query-urile.
Logica de business ramane intr-un singur loc, iar UI web si API folosesc acelasi nucleu.

## Cum ajuta pentru o aplicatie mobila pe acelasi proiect?
Aplicatia mobila ar consuma API-ul, iar API-ul ar folosi aceeasi structura service + repository.
Asta inseamna:
- acelasi comportament de business pe web si mobil
- mai putine bug-uri din logica duplicata
- evolutie mai rapida, pentru ca modificarile se fac centralizat in servicii/repository-uri
