Ladda ned repo:  git@github.com:Max-comerit/Blazor-CoHosted-Demo.git  

Uppdatera appsettings.json i src/Companies.API till er localdb sträng  

Bygg solution    

Högerklicka projektet Compani.Api och, välj Manage user-secrets och sett dem till:   
{
  "password": "password",
  "secretkey" :"ThisMustBeReallylongXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXZZZZ"
}


Gå till packet-manager, set default project till src\Companies.API  

Kör: Update-Database

Öppna SQL Server Object Explorer och Hitta er DB  

Gå till Databases\System Databases\Master\Tables  

Högerklicka dbo.AspNetUsers -> View data  

Välj användarnamn till en godtycklig användare, har format namn+siffra, t.ex. Max34  


Gå till project TestWithBasicAuth i lösningen  

I @code, Pages\Counter Login-funktion finns en hårdkodad användare  

Sätt användarnamnet ni hittade i er DB, med lösenord password  

Klart.
