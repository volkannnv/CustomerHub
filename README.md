# CustomerHub

Visual Studio’da ASP.NET Core Empty projesi oluşturup başladım. Program.cs dosyasında MVC, oturum (session) yönetimi gibi temel servisleri ekledim.
SQL Server’da "CustomerHub" adında bir veritabanı oluşturdum ve Users, Customers, UserCustomers, CustomerAddresses ve CustomerPhones tablolarını ekledim.
Tabloları oluştururken kafamda User ve Customer arasında bir many to many ilişkisi olur diye kurmuştum fakat son hali öyle değil. Bir Customer hali hazırda varsa başka bir user'ın onu yaratmasına gerek olmasın aynı customer için databasede bir enrty daha açılmasın diye düşünmüştüm ama şu anki halinde tabiri caizse herkesin müşterisi kendine
Projede temel veri işlemlerini gerçekleştirmek için DatabaseHelper.cs dosyasını yazmaya başladım. Validate ve CRUD işlemlerini ekledim. 
Projeyi ilk kaldırdığımda user farketmeksizin bütün customerları getiriyordu, öyle şey tabiki olmaz hemen değiştirdim.
UI'da bir sürü pop-up var, adres ve telefon numaralarının görüntülenmesini nasıl sağlamam gerekiyordu tam hatırlayamadım popup olarak yaptım baktım tüm projenin bitmesi çok sürmeyecek en kötü nasıl olması gerekiyorsa sonra değiştiririm diye düşündüm. ilk konuştuğumuz perşembe günü projeye bakamadım. cuma başladım ve cumartesinin sonunda nerdeyse bitmişti, bugün pazar ve yine sabahtan dışarı çıkmam gerekti ve akşam döndüm ve rütuşları attım ve şu an readme yazıyorum sonrasında kısa bir demo videosu da çekeceğim.
proje geneli çok sıkı bir denetim yok, şifre güvenliği olsun telefon numaralarının formatı olsun adres olsun. bir an önce elimde gösterebileceğim bir şey olması için es geçtim
onun haricinde sanırım konuşulmamıştı ama kullanıcıların username ve şifre değştirebilecekleri bir kullanıcı sayfası da yaptım. 
repo'yu açmamın ana sebebi olur da denk gelmez de uzaktan bağlanamazsanız diye kodları inceleyebilmeniz için, demo videosundan sonra kendi local'inizde deneyebilmeniz için database'i oluştururken kullandığım query'yi de bir txt dosyası yapıp pushlayacağım. en kötü ihtimal unutursam whatsapp'tan da iletebilirim. 

Döküman yazmaya alışık değilim, formatı ve üslubum uygun değilse bu seferlik mazur görün lütfen. tekrardan ilgilendiğiniz için teşekkür ederim
