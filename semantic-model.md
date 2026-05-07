# Semantic Model

Sažeti semantički model baze podataka za aplikaciju EscapeRoomReviews.

## Pregled modela i tablica

| Model / klasa | Tablica | Svrha |
| --- | --- | --- |
| User | Users | Korisnici aplikacije i autori recenzija |
| Company | Companies | Tvrtke koje posjeduju escape roomove |
| Location | Locations | Lokacije na kojima se escape roomovi nalaze |
| EscapeRoom | EscapeRooms | Glavni entitet za pojedini escape room |
| Photo | Photos | Fotografije povezane s escape roomom |
| Review | Reviews | Recenzije koje korisnici pišu za escape roomove |
| Theme | Themes | Tematske oznake escape roomova |
| EscapeRoomTheme | EscapeRoomTheme | Spojna tablica za many-to-many vezu između EscapeRoom i Theme |

## Glavna svojstva po modelu

### User
- Id
- Username
- Email
- RegisteredAt
- TotalRoomsPlayed
- Role
- Veza: jedan korisnik ima više recenzija

### Company
- Id
- Name
- Website
- Veza: jedna tvrtka ima više escape roomova

### Location
- Id
- City
- Address
- PostalCode
- Latitude
- Longitude
- Veza: jedna lokacija ima više escape roomova

### EscapeRoom
- Id
- Name
- Description
- Difficulty
- MaxPlayers
- Price
- CreatedAt
- LocationId
- CompanyId
- Veze: pripada jednoj lokaciji, jednoj tvrtki, ima više fotografija, više recenzija i više tema

### Photo
- Id
- Url
- EscapeRoomId
- Veza: svaka fotografija pripada jednom escape roomu

### Review
- Id
- Rating
- Comment
- CreatedAt
- IsVerified
- PlayersCount
- EscapeRoomId
- UserId
- Veze: svaka recenzija pripada jednom escape roomu i jednom korisniku

### Theme
- Id
- Name
- IconUrl
- Veza: jedan theme može biti povezan s više escape roomova

## Ključne veze između tablica

- User 1:N Review
  - jedan korisnik može napisati više recenzija
  - svaka recenzija ima točno jednog autora

- Company 1:N EscapeRoom
  - jedna tvrtka može imati više escape roomova
  - svaki escape room pripada jednoj tvrtki

- Location 1:N EscapeRoom
  - jedna lokacija može sadržavati više escape roomova
  - svaki escape room pripada jednoj lokaciji

- EscapeRoom 1:N Photo
  - jedan escape room može imati više fotografija
  - svaka fotografija pripada jednom escape roomu

- EscapeRoom 1:N Review
  - jedan escape room može imati više recenzija
  - svaka recenzija se odnosi na jedan escape room

- EscapeRoom N:M Theme
  - jedan escape room može imati više tema
  - jedna tema može biti povezana s više escape roomova
  - relacija se realizira preko spojne tablice EscapeRoomTheme

## Enum polja

- DifficultyLevel: Easy, Medium, Hard, Expert
- UserRole: User, Moderator, Admin

## Napomena o ponašanju brisanja

- Brisanje Location briše povezane EscapeRoom zapise.
- Brisanje Company briše povezane EscapeRoom zapise.
- EscapeRoom je roditelj za Photo i Review zapise kroz relacije definirane u DbContextu.