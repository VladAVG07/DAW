# README

## 2. Intrebari conceptuale

### 1) De ce Logout este implementat ca `<form method="post">` si nu ca un link `<a href="/Auth/Logout">`?
Logout modifica starea sesiunii utilizatorului, deci este o operatie de tip state-changing. Din acest motiv trebuie facut prin POST si protejat cu antiforgery token.

Daca logout ar fi pe GET, un site extern ar putea forta utilizatorul sa faca logout fara intentia lui, de exemplu printr-un link sau o imagine care loveste `/Auth/Logout`. Asta este un risc de tip CSRF.

### 2) De ce login-ul face doi pasi in loc de unul?
In implementarea facuta, autentificarea incepe cu cautarea utilizatorului dupa email, apoi parola este verificata cu `PasswordSignInAsync` folosind `UserName`.

`Email` si `UserName` sunt campuri diferite in Identity. `Email` este adresa de contact, iar `UserName` este identificatorul de autentificare folosit implicit de multe API-uri din Identity.

De aceea:
- pasul 1: `FindByEmailAsync(model.Email)`
- pasul 2: `PasswordSignInAsync(user.UserName!, model.Password, ...)`

### 3) De ce nu este suficient sa ascunzi butoanele Edit/Delete in View?
Ascunderea butoanelor in View este doar UX, nu securitate. Un utilizator poate accesa direct URL-ul `/Articles/Edit/5` sau poate trimite request manual.

De aceea avem verificare reala in backend:
- `[Authorize]` pentru autentificare
- `IsOwnerOrAdmin()` pentru drepturi de ownership/rol

Daca am pune doar `[Authorize]` in controller si nu am ascunde butoanele in View, functional ar fi corect si securizat, dar experienta utilizatorului ar fi slaba: ar vedea actiuni pe care nu le poate executa si ar primi `403` dupa click.

### 4) Ce este middleware pipeline-ul in ASP.NET Core?
Middleware pipeline este lantul de componente prin care trece fiecare request HTTP. Fiecare middleware poate prelucra request-ul, poate adauga date in context si poate decide daca paseaza request-ul mai departe.

`UseAuthentication()` trebuie sa fie inainte de `UseAuthorization()` pentru ca autorizarea are nevoie de identitatea utilizatorului deja construita.

Daca le inversam:
- autorizarea ruleaza inainte sa existe utilizator autentificat in context
- politicile/rolurile nu se evalueaza corect
- request-uri valide pot fi respinse incorect

### 5) Ce am fi implementat manual daca nu foloseam ASP.NET Core Identity?
Fara Identity, ar fi trebuit sa implementam manual:
- model de utilizator si schema pentru roluri/claims
- stocare parola cu hashing sigur (ex: PBKDF2/Argon2), salt, eventual pepper
- validare parola si comparare sigura
- management cookie/sesiune, sign-in/sign-out
- protectie CSRF, lockout, reset parola, confirmare email
- reguli de autorizare pe roluri si ownership
- hardening de securitate si testare pentru toate acestea

Practic, am fi rescris multe mecanisme critice de securitate, cu risc mare de erori.

### 6) Care sunt dezavantajele folosirii ASP.NET Core Identity?
Identity este foarte util, dar are compromisuri:
- vine cu schema proprie de tabele si conventii, mai greu de adaptat in sisteme legacy
- creste complexitatea proiectului pentru cazuri foarte simple
- migrarea catre alt provider auth poate fi costisitoare
- pentru API-first/mobile/frontend separat, cookie auth nu este de obicei ideal si trebuie extins cu token/JWT/OIDC
- daca vrei un model user complet custom, uneori configurarea devine mai complexa

In schimb, pentru aplicatii web clasice MVC, avantajul principal ramane securitatea buna out-of-the-box.
