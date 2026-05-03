using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace BlogApp.Data.Concrete.EFCore
{
    // ************************* Seed Data (Test Verileri) ***************************
    // Database bağlantısı yapılıp, migration'la database oluşturulduktan sonra, bu aşamada database'i test etmek için, table'ların içerisinde bir veri bulunmayacak. Bunu içinde seed data'yı kullanıyoruz.
    // Seed data ile static bir class'ın ve static bir void metodun içerisinde, IApplicationBuilder interface'i üzerinden, bir ApplicationServices çağırarak, CreateScope() metodu ile bir Scope yaşam süresi oluşturup, ServiceProvider'a erişip, GetService metodunun generic'ine Context class'ımızı göndererek, bu DbContext'in instance'ını alabiliriz. Yada istediğimiz class'ın.
    // Bu IApplicationBuilder interface ile bu açtığımız statik metodun parametresi üzerinden, IoC Container'dan var app = Builder.Build() satırındaki, bize IServiceProvider'ı veren ApplicationBuilder türündeki app değişkenini talep diyoruz. Nasıl? Service provider'a gidip bu class'ı ve bu metodu çağırıp, app değişkenini parametre olarak gönderiyoruz.
    // var app = builder.Build();
    //SeedData.TestVerileriniDoldur(app); 

    public static class SeedData // Bu class'ını static olmasının sebebi program çalıştıktan hemen sonra, cache'te bu class'ın instance'ı kayıt olup, uygulama kapanan kadar o instance kullanılabilmesi için, migration'ları güncellemek ve eğer veri yoksa test verilerini database'e eklemek içindir.  
    {
        public static void TestVerileriniDoldur(IApplicationBuilder app)
        {
            var context = app.ApplicationServices.CreateScope ().ServiceProvider.GetRequiredService<BlogContext>();
            if (context != null) 
            {
                if (context.Database.GetPendingMigrations().Any()) // Burada şunu yapmış olduk: Dedik ki bu projede uygulanmış ama update edilmemiş herhangi bir migration varsa, bu koşulu true dön ve bu bloğa gir. (GetPendingMigrations: bekleyen migrationları al). 
                {
                    context.Database.Migrate(); // Burada da bir daha update-database komutunu çalıştırmayalım diyerek, program her çalıştırıldığında, update edilmemiş migration varsa, otomatik olarak database'i update eder.
                      // Uygulama bu bloğa her girdiğinde database'i update etmiş olur. 
                }

                // Artık bu kısımdan sonra, eğer database'in tablolarında herhangi bir veri yoksa, test etmek için oluşturulmuş verileri database'e ekleme işlemini yapıyoruz.

                if (!context.Tags.Any()) // Burada Eğer burada verilen table'ın içinde herhangi bir veri yoksa, bu bloğa gir demiş olduk.
                {
                    // Bu table'ye test verilerini ekleyip kaydettik.
                    context.Tags.AddRange(
                        new Entity.Tag { Text = "Travel", Url = "traval", Color = Entity.TagColors.warning },
                        new Entity.Tag { Text = "Places to See", Url = "places-to-see", Color = Entity.TagColors.danger },
                        new Entity.Tag { Text = "Civilizations", Url = "civilizations", Color = Entity.TagColors.primary },
                        new Entity.Tag { Text = "Europa", Url = "europa", Color = Entity.TagColors.success },
                        new Entity.Tag { Text = "Travel Guide", Url = "travel-guide", Color = Entity.TagColors.secondary }
                    );
                    context.SaveChanges();
                }

                if (!context.Users.Any())
                {
                    context.Users.AddRange(
                        new Entity.User { UserName = "mehmetyildiz",Name = "Mehmet Yıldız", Email = "info@mehmetyildiz.com", Password = "123456", Image = "p1.jpg"},
                        new Entity.User { UserName = "alisahin",Name = "Ali Şahin", Email = "info@alisahin.com", Password = "123456", Image = "p2.jpg"}
                        );
                    context.SaveChanges();
                }

                if (!context.Posts.Any())
                {
                    context.Posts.AddRange(
                        new Entity.Post
                        {
                            Title = "NYC: The City of Endless Energy",
                            Description = "New York City isn’t just a place on a map; it’s an experience, a relentless pulse of human energy that captures your soul the moment you arrive.",
                            Content = "New York City isn’t just a place on a map; it’s an experience, a relentless pulse of human energy that captures your soul the moment you arrive. Widely known as \"The Big Apple,\" NYC stands as a global beacon of culture, commerce, and creativity, offering something for every kind of traveler.\r\n\r\nFor first-time visitors, the sheer scale of the city is awe-inspiring. A walk through Times Square, with its blinding neon lights and digital billboards, places you right at the \"Crossroads of the World.\" But the city’s true magic lies in its diversity. Each borough has its own distinct personality. You can wander through the artistic streets of Greenwich Village, enjoy authentic cuisine in Little Italy or Chinatown, or feel the historical gravity of Wall Street in Lower Manhattan.\r\n\r\nOf course, no trip to NYC is complete without seeing its iconic landmarks. The architectural marvel of the Empire State Building offers breathtaking views of the sprawling urban landscape. A ferry ride to the Statue of Liberty reminds you of the city's rich history as a gateway for millions of immigrants. And for a necessary escape from the urban concrete, the vast, green expanse of Central Park provides a serene oasis right in the heart of the city.\r\n\r\nNew York is also a world-class hub for the arts. Whether you catch a dazzling Broadway show, explore the culinary genius found in thousands of restaurants, or get lost in the halls of the Metropolitan Museum of Art, the city is a constant celebration of human expression.\r\n\r\nThey say it’s the \"City That Never Sleeps,\" and it’s true. From its efficient, 24-hour subway system to the late-night jazz clubs, NYC is alive at all hours. It is a place where cultures collide, trends are born, and dreams are pursued with unmatched passion. New York City doesn't just invite you to visit; it challenges you to keep up with its extraordinary pace.",
                            Url = "nyc",
                            IsActive = true,
                            IsDelete = false,
                            PublishedOn = DateTime.Now.AddDays(-10),
                            Tags = context.Tags.Take(3).ToList(),
                            Image = "1.jpg",
                            UserId = 1,
                            //User = context.Users.First(),
                            Comments = new List<Entity.Comment>
                            {
                                new Entity.Comment { Text = "Amazing a city", PublishedOn = DateTime.Now.AddHours(-10), UserId = 1 },
                                new Entity.Comment { Text = "I want to go there Tavsiye ederim", PublishedOn = DateTime.Now, UserId = 2 },
                            }
                        },
                        new Entity.Post
                        {
                            Title = "The Eternal City of Time and Grandeur",
                            Description = "Rome, often called \"The Eternal City,\" is a living museum where nearly three thousand years of history coexist beautifully with modern life.",
                            Content = "Rome, often called \"The Eternal City,\" is a living museum where nearly three thousand years of history coexist beautifully with modern life. As the former heart of the vast Roman Empire and the cradle of ancient civilization, Rome offers an unparalleled journey into the past, wrapping visitors in a unique blend of awe-inspiring ruins, vibrant piazzas, and artistic masterpieces.\r\n\r\nFor first-time visitors, walking through Rome feels like stepping into a time machine. The majestic Colosseum, the site of gladiatorial battles, stands as a massive monument to the empire's engineering prowess and dramatic history. Just steps away, the Roman Forum provides a glimpse into ancient civic life, with columns and temples whispering stories of emperors and senators.\r\n\r\nThe city’s spiritual heart is found in Vatican City, an independent state within Rome. St. Peter's Basilica, with its breathtaking dome designed by Michelangelo, and the Vatican Museums, housing masterpieces like the Sistine Chapel, are unparalleled artistic and religious landmarks. Yet, much of Rome's charm is found in its simpler moments: tossing a coin into the spectacular Trevi Fountain to ensure a return visit, wandering through the narrow, cobblestone streets of the Trastevere district, or enjoying a slow 'aperitivo' in a sun-drenched piazza.\r\n\r\nRoma is also a culinary capital, celebrating simple yet profound flavors. Sampling authentic pasta carbonara or cacio e pepe, followed by a scoop of gelato, is as essential as any historical tour. From the stunning dome of the Pantheon to the iconic Spanish Steps, Rome invites exploration and contemplation, proving that true grandeur doesn't fade, it simply becomes eternal.",
                            Url = "roma",
                            IsActive = true,
                            IsDelete = false,
                            PublishedOn = DateTime.Now.AddDays(-20),
                            Tags = context.Tags.Take(3).ToList(),
                            Image = "2.jpg",
                            UserId = 1
                            //User = context.Users.First(),
                        },
                        new Entity.Post
                        {
                            Title = "Vienna: The Imperial Capital of Dreams and Melodies",
                            Description = "Vienna, the capital of Austria, is a city where history is not just remembered; it is gracefully lived. Situated on the banks of the Danube, this former heart of the vast Austro-Hungarian Empire remains a global beacon of imperial grandeur, classical music, and sophisticated living.",
                            Content = "Vienna, the capital of Austria, is a city where history is not just remembered; it is gracefully lived. Situated on the banks of the Danube, this former heart of the vast Austro-Hungarian Empire remains a global beacon of imperial grandeur, classical music, and sophisticated living. Walking through Vienna’s streets, one is wrapped in an atmosphere that is simultaneously grand and intimate, moving effortlessly between Baroque palaces and cozy, historical coffee houses.\r\n\r\nFor first-time visitors, the sheer architectural magnificence of the city center, a UNESCO World Heritage site, is awe-inspiring. The sprawling Hofburg Palace, the winter residence of the Habsburgs, offers a journey into the life of Empress Sisi and the sheer power of an empire. Equally stunning is the Schönbrunn Palace, the Habsburg’s summer residence, with its endless gardens and Gloriette offering panoramic views. But the defining silhouette of Vienna is St. Stephen's Cathedral, its multi-colored tiled roof standing proudly above the historic heart.\r\n\r\nVienna’s global identity is inseparable from classical music. It was the home of Mozart, Beethoven, Strauss, and Mahler, and their spirits live on in the world-renowned Vienna State Opera and the elegant Musikverein. Beyond the music and the monarchy, Vienna offers a world-famous coffee house culture. Spending an afternoon in a place like Café Central, enjoying a cup of Melange and a slice of Sachertorte, is not just about the food; it is an immersion into a unique, slow-paced Viennese institution. Vienna invites exploration, demanding a pace that allows one to appreciate both its monumental past and its charming, melody-filled present.",
                            Url = "vienna",
                            IsActive = true,
                            IsDelete = false,
                            PublishedOn = DateTime.Now.AddDays(-30),
                            Tags = context.Tags.Take(4).ToList(),
                            Image = "3.jpg",
                            UserId = 2
                            //User = context.Users.First(),
                        },
                        new Entity.Post
                        {
                            Title = "London: A Timeless Journey Through Fog and Fortune",
                            Description = "London, the majestic capital of the United Kingdom, is a city where every street corner feels like a page from a history book, yet the energy is undeniably modern.",
                            Content = "London, the majestic capital of the United Kingdom, is a city where every street corner feels like a page from a history book, yet the energy is undeniably modern. From the royal traditions of the monarchy to the cutting-edge art scenes of the East End, London offers a diverse experience that blends centuries of heritage with a fast-paced, global lifestyle.\r\n\r\nFor first-time explorers, the city’s landmarks provide an iconic skyline. The Tower of London stands as a powerful reminder of the city's medieval past, while just across the river, the Shard pierces the clouds as a symbol of contemporary ambition. No visit is complete without witnessing the architectural glory of the Houses of Parliament and the world-famous Big Ben, whose chimes have signaled the heartbeat of the nation for generations.\r\n\r\nLondon is also a city of royal elegance. A walk through St. James’s Park leads you to the gates of Buckingham Palace, where the Changing of the Guard ceremony continues to captivate crowds. But to truly feel the city’s pulse, you must head to its vibrant neighborhoods. From the upscale boutiques of Bond Street to the eclectic, colorful stalls of Camden Market or the cinematic charm of Notting Hill, London is a collection of villages, each with its own soul.\r\n\r\nCulturally, the city is a powerhouse. You can spend days lost in the treasures of the British Museum or admiring modern masterpieces at the Tate Modern. As the sun sets, the neon lights of Piccadilly Circus lead the way to the West End, where world-class theater productions bring stories to life every night. Whether you’re enjoying a traditional afternoon tea or exploring the diverse culinary delights of Borough Market, London challenges you to see the world from a different perspective. It is a city that has mastered the art of honoring its past while fearasting on the future.",
                            Url = "londra",
                            IsActive = true,
                            IsDelete = false,
                            PublishedOn = DateTime.Now.AddDays(-40),
                            Tags = context.Tags.Take(5).ToList(),
                            Image = "4.jpg",
                            UserId = 2
                            //User = context.Users.First(),
                        },
                        new Entity.Post
                        {
                            Title = "Amsterdam: The Venice of the North and the City of Canals",
                            Description = "Amsterdam, the capital of the Netherlands, is a city that feels like a masterpiece of urban design and historical charm. Famous for its intricate canal system, narrow gabled houses, and a culture centered around bicycles, Amsterdam offers a unique blend of 17th-century Golden Age atmosphere and a modern, progressive spirit.",
                            Content = "Amsterdam, the capital of the Netherlands, is a city that feels like a masterpiece of urban design and historical charm. Famous for its intricate canal system, narrow gabled houses, and a culture centered around bicycles, Amsterdam offers a unique blend of 17th-century Golden Age atmosphere and a modern, progressive spirit.\r\n\r\nFor first-time visitors, the best way to experience the city is by water. A canal cruise through the Grachtengordel (the Canal Belt) reveals the stunning architecture of the merchant houses that have stood for centuries. But Amsterdam is truly a city of wheels; thousands of cyclists navigating the cobblestone streets create a rhythmic energy that is uniquely Dutch.\r\n\r\nThe city is a global treasure trove for art and history lovers. The Museumplein is home to the world-renowned Rijksmuseum, which houses masterpieces by Rembrandt and Vermeer, and the Van Gogh Museum, dedicated to the life and works of the legendary artist. For a more somber and reflective experience, the Anne Frank House tells a powerful story of human resilience and history that resonates with every visitor.\r\n\r\nBeyond the museums, Amsterdam’s neighborhoods each offer a different vibe. The Jordaan district is perfect for wandering through narrow streets filled with independent boutiques and cozy pubs known as \"brown cafés.\" In the spring, the city explodes with color as the nearby tulip fields bloom, adding to the city's reputation as a vibrant hub of life. Whether you’re exploring the flower markets, enjoying a \"stroopwafel\" by the water, or relaxing in the vast Vondelpark, Amsterdam invites you to slow down and enjoy the simple pleasures of a city built on water and light.",
                            Url = "amsterdam",
                            IsActive = true,
                            IsDelete = false,
                            PublishedOn = DateTime.Now.AddDays(-50),
                            Tags = context.Tags.Take(4).ToList(),
                            Image = "5.jpg",
                            UserId = 2
                            //User = context.Users.First(),
                        },
                        new Entity.Post
                        {
                            Title = "Istanbul: The Soul of Two Continents and the Eternal Crossroads",
                            Description = "Istanbul isn't just a city; it’s a living museum where centuries of history blend beautifully with modern Turkish life.",
                            Content = "Istanbul isn't just a city; it’s a living museum where centuries of history blend beautifully with modern Turkish life. As the former capital of both the Byzantine and Ottoman Empires, this dynamic metropolis stands as a global crossroads, physically and culturally bridging Europe and Asia across the majestic Bosphorus Strait. Walking through Istanbul’s streets, one is wrapped in an atmosphere that is simultaneously grand and intimate, moving effortlessly between ancient mosques and trendy, cosmopolitan neighborhoods.\r\n\r\nFor first-time explorers, the city’s heart, the historic Sultanahmet district, offers breathtaking landmarks. The majestic Hagia Sophia Grand Mosque, with its massive dome, stands as a testament to two millennia of architectural genius. Just across the square, the ornate columns and minarets of the Blue Mosque define the skyline. A few steps away, the sprawling Topkapi Palace provides a glimpse into the opulent world of Ottoman sultans. Yet, much of Istanbul's true magic lies in its vibrant, chaotic markets, where you can wander through the Grand Bazaar, one of the oldest and largest covered markets, and lose yourself in a dazzling array of spices, textiles, and authentic crafts.\r\n\r\nculturally, Istanbul is a powerhouse. You can spend days exploring art galleries in Karaköy or admiring the treasures of the Archeological Museums. For a necessary escape from the urban energy, a traditional Bosphorus Cruise offers spectacular views of Ottoman palaces, wooden yalis, and the city’s two major bridges connecting the two sides. As the sun sets, a walk across the Galata Bridge, with its line of fishermen and iconic tower, leads you to Beyoglu, where the vibrant pedestrian street of Istiklal Avenue leads to Taksim Square, the heart of modern Istanbul. Beyond the monuments, Istanbul is a culinary capital, celebrating rich and diverse flavors. Sampling authentic Turkish Meze or a simple yet profound cup of strong Turkish tea, followed by a slice of baklava or Turkish delight, is as essential as any historical tour.\r\n\r\nThey say Istanbul is a city that never sleeps, and it’s true. It is a place where cultures collide, history is whispered by every ancient wall, and life is pursued with unmatched passion. Istanbul doesn't just invite you to visit; it challenges you to embrace its extraordinary blend of East and West and find your own tempo within its relentless, beautiful rhythm.",
                            Url = "istanbul",
                            IsActive = true,
                            IsDelete = false,
                            PublishedOn = DateTime.Now.AddDays(-60),
                            Tags = context.Tags.Take(5).ToList(),
                            Image = "6.jpg",
                            UserId = 1
                            //User = context.Users.First(),
                        }
                        );
                    context.SaveChanges();
                }
            }

        } 
    }
}
