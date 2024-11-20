class Info{
    Guid id;
    DateTime createdDate;
    public Guid Id{ get; set; }
    public DateTime CreatedDate{ get; set; }
    public Info(DateTime? createdDate = null){
        Id = Guid.NewGuid(); 
        CreatedDate = createdDate ?? DateTime.Now;        
    }
    public virtual void DisplayInfo(){
        Console.WriteLine($"{id}, {createdDate}");  
    }
}
class Books : Info{
    string title;
    public string Title{ get{ return title; } set{ title = value; } }
    public Books(string title, DateTime? createdDate = null) : base(createdDate){
        Title = title;
    }
    public override void DisplayInfo(){
        Console.WriteLine($"ID: {base.Id}, Title: {title}, Created Date: {base.CreatedDate}");  
    }
}
class Users : Info{
    string name;
    public string Name{get; set;}
    public Users(string name, DateTime? createdDate = null) : base(createdDate){
        Name = name;
    }
    public override void DisplayInfo(){
        Console.WriteLine($"ID: {base.Id}, Name: {Name}, Created Date: {base.CreatedDate}");  
    }
}
interface INotificationService{
    public void SendNotificationOnSucess();
    public void SendNotificationOnFailure();
}
class EmailNotificationService :  INotificationService{
    public void SendNotificationOnFailure(){
        Console.WriteLine($"We encountered an issue adding. \n Please review the input data. For more help, visit our FAQ at library.com/faq.");  
    }
    public void SendNotificationOnSucess(){
        Console.WriteLine($"Successfully added to the system. \n If you have any queries or feedback, please contact our support team at support@library.com.");
    }
}
class SMSNotificationService :  INotificationService{
    public void SendNotificationOnFailure(){
        Console.WriteLine($"Error adding information. Please email support@library.com.");
    }
    public void SendNotificationOnSucess(){
        Console.WriteLine($"Information added to the System. Thank you!");
    }
}    
class Library{ 
    readonly List<Books> books = [];
    readonly List<Users> users = [];
    INotificationService notificationService;
    public Library(INotificationService notificationService){
        this.notificationService = notificationService;
    }
    public List<T> GetValues<T>(List<T> list, int pageNumber = 1, int limitPerPage = 3){
        return list.Skip((pageNumber - 1) * limitPerPage).Take(limitPerPage).ToList();
    }
    public void GetBooks(){
       var displayListOfBooks = GetValues(books, 1, 3).OrderByDescending(i => i.CreatedDate);
       foreach(var i in displayListOfBooks){
            i.DisplayInfo();
       }
    }
    public void GetUsers(){
       var displayListOfUsers = GetValues(users, 1, 3).OrderByDescending(i => i.CreatedDate);
       foreach(var i in displayListOfUsers){
            i.DisplayInfo();
       }
    }
    public void FindBooksByTitle(string title){
        List<Books> listBooks = books.FindAll(i => i.Title.Contains(title, StringComparison.OrdinalIgnoreCase)).ToList();
        listBooks.ForEach(i => i.DisplayInfo());       
    }
    public void FindUsersByName(string name){
        List<Users> listUsers = users.FindAll(i => i.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
        listUsers.ForEach(i => i.DisplayInfo());
    }
    public void AddBook(Books book){
        if(!books.Exists(i => i.Title.Contains(book.Title)) && notificationService != null){
            books.Add(book);
            notificationService?.SendNotificationOnSucess();  
        }else{
            notificationService?.SendNotificationOnFailure();
        }  
    }
    public void AddUser(Users user){
        if(!users.Exists(i => i.Name.Contains(user.Name, StringComparison.OrdinalIgnoreCase))){
            users.Add(user);
            notificationService?.SendNotificationOnSucess();
        }else{  
            notificationService?.SendNotificationOnFailure();
        }  
    }
    public void DeleteUser(Guid id){
        Users findUser = users.Find(i => i.Id == id);
        if(findUser != null){
            users.Remove(findUser);
            Console.WriteLine($"User removed"); 
        }else{
            Console.WriteLine($"User not found");  
        }    
    }
    public void DeleteBook(Guid id){
        Books findBook = books.Find(i => i.Id == id);
        if(findBook != null){
            books.Remove(findBook);
            Console.WriteLine($"Book removed");    
        }else{
            Console.WriteLine($"Book not found");   
        }
    }
}
internal class Program
{
    public static void Main()
    {
        var user7 = new Users("George", new DateTime(2024, 7, 1));
        user7.DisplayInfo();
        var user8 = new Users("Hannah", new DateTime(2024, 8, 1));
        var user9 = new Users("Ian");
        var user10 = new Users("Julia");
        user10.DisplayInfo();
        
        var book1 = new Books("The Great Gatsby", new DateTime(2023, 1, 1));
        book1.DisplayInfo();
        var book2 = new Books("1984", new DateTime(2023, 2, 1));
        var book19 = new Books("The Iliad");
        book19.DisplayInfo();
        var book20 = new Books("Anna Karenina");
        var emailService = new EmailNotificationService();
        var smsService = new SMSNotificationService();
        var libraryWithEmail = new Library(emailService);
        var libraryWithSMS = new Library(smsService);

        libraryWithEmail.AddBook(new Books("anna karenina"));
        libraryWithSMS.AddUser(new Users("Almaha"));
        libraryWithEmail.AddBook(book2);
        libraryWithEmail.AddUser(user10);
        libraryWithSMS.AddUser(new Users("raghad", new DateTime(2023, 2, 1)));
        libraryWithEmail.DeleteBook(book19.Id);
        libraryWithSMS.DeleteUser(user10.Id);        
        libraryWithEmail.FindBooksByTitle("Anna Karenina");
        libraryWithSMS.FindUsersByName("Almaha");
        libraryWithEmail.GetBooks();
        libraryWithEmail.GetUsers();

    
    }
}