using MyLinqTask;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace MyLinqTask
{
    class User
    {
        public int ID { get; set; }
        public String Name { get; set; }
        public String Surname { get; set; }
        public User(int id, String name, String surname)
        {
            this.ID = id;
            this.Name = name;
            this.Surname = surname;
        }
        public override string ToString()
        {
            return string.Format("ID={0}: {1} {2}", ID, Name, Surname);
        }
    }
    class Record
    {
        public User Author { get; set; }
        public String Message { get; set; }
        public Record(User author, String message)
        {
            this.Author = author;
            this.Message = message;
        }
    }
    class BusinessLogic
    {
        private List<User> users = new List<User>();
        private List<Record> records = new List<Record>();
        public BusinessLogic()
        {
            // наполнение обеих коллекций тестовыми данными
            User Vadim = new User(0, "Vadim", "Ploskarev");
            User Alexey = new User(1, "Alexey", "Rybolovlel");
            User Alex = new User(2, "Alex", "Yavorovsky");
            User Artem = new User(3, "Artem", "Burashnikov");
            User Polina = new User(4, "Polina", "Savelyeva");
            users.Add(Vadim); users.Add(Alexey); users.Add(Alex); users.Add(Artem); users.Add(Polina);



            Record Mess1 = new Record(Vadim, "I like python");
            Record Mess2 = new Record(Alexey, "I also like python");
            Record Mess3 = new Record(Alex, "My name is Sasha");
            Record Mess4 = new Record(Artem, "I didn't do geometry homework");
            Record Mess5 = new Record(Polina, "I have missed lessons today");
            records.Add(Mess1); records.Add(Mess2); records.Add(Mess3); records.Add(Mess4); records.Add(Mess5);
        }

        //We want to get list of users by their surname
        public List<User> GetUsersBySurname(String surname)
        {
            List<User> users_with_this_surname = (from us in users where us.Surname == surname select us).ToList();
            return users_with_this_surname;
        }

        //we want to get user by his ID. If we get 2 or more user --> exception ( using .Single() ) 
        public User GetUserByID(int id)
        {
            var user_by_id = (from us in users where us.ID == id select us).Single();
            return user_by_id;
        }

        //we want to get user by substring (using select with Contains() ) 
        public List<User> GetUsersBySubstring(String substring)
        {
            var user_by_substring = (from us in users where ( us.Name.Contains(substring) || us.Surname.Contains(substring) ) select us).ToList();
            return user_by_substring;
        }

        //get all unique names using Distinct 
        public List<String> GetAllUniqueNames()
        {
            var users_with_unique_name = (from us in users select us.Name).Distinct().ToList();
            return users_with_unique_name;
        }

        //get all authors 
        public List<User> GetAllAuthors()
        {
            var all_authors = (from rec in records select rec.Author).ToList();
            return all_authors;
        }

        //make users dictionary 
        public Dictionary<int, User> GetUsersDictionary()
        {
            var users_dictionary = users.ToDictionary(k => k.ID,k => k);
            return users_dictionary;
        }

        //find max id
        public int GetMaxID()
        {
            int max_id = (from i in users select i.ID).Max();
            if (max_id >= 0)
            {
                return max_id;
            }
            return -1;
        }

        //sort users by id
        public List<User> GetOrderedUsers()
        {
            var sort_users = (from us in users orderby us.ID select us).ToList();
            return sort_users;

        }


        //reverse sort users by id
        public List<User> GetDescendingOrderedUsers()
        {
            var sort_users = (from us in users orderby  us.ID descending select us).ToList();
            return sort_users;

        }


        //reverse list
        public List<User> GetReversedUsers()
        {
            var reverse_users = (from us in users select us).Reverse().ToList();
            return reverse_users;
        }


        //using take() and skip()
        public List<User> GetUsersPage(int pageSize, int pageIndex)
        {
            var users_page = (from us in users select us).Skip(pageSize*pageIndex).Take(pageSize).ToList();
            return users_page;
        }

    }

    class Program : BusinessLogic
    {
        static void Main(string[] args)
        {
            BusinessLogic businessLogic = new BusinessLogic();

            //test get users by surname  1
            List<User> first_test = businessLogic.GetUsersBySurname("Ploskarev");

            foreach (var user in first_test) {
                Console.WriteLine("test Get Users By Surname - Ploskarev");
                Console.WriteLine(user.Name); }
                Console.WriteLine();

            //test GetUserByID  2
            User second_test = businessLogic.GetUserByID(2);

            Console.WriteLine("test GetUserByID - 2");
            Console.WriteLine(second_test.Name + second_test.Surname);
            Console.WriteLine();

            //test GetUsersBySubstring  3
            List<User> third_test = businessLogic.GetUsersBySubstring("i");

            Console.WriteLine("test GetUsersBySubstring - i");
            foreach (var user in third_test)
            {
                Console.Write(user.Name + user.Surname + " ");             
            }
            Console.WriteLine();


            //test GetAllUniqueNames  4
            List<String> four_test = businessLogic.GetAllUniqueNames();
            Console.WriteLine("test GetAllUniqueNames ");

            foreach (var user in four_test)
            {
                Console.Write(user + " ");
            }
            Console.WriteLine();


            //test GetAllAuthorse  5
            List<User> five_test = businessLogic.GetAllAuthors();
            Console.WriteLine("test GetAllAuthors ");

            foreach (var user in five_test)
            {
                Console.Write(user.Name + user.Surname + " ");
            }
            Console.WriteLine();


            //test GetUsersDictionary  6
            var six_test = businessLogic.GetUsersDictionary();
            Console.WriteLine("test GetUsersDictionary ");

            foreach (var user in six_test)  
            {
                Console.Write(user.Key + user.Value.Surname + " ");
            }
            Console.WriteLine();


            //test GetMaxID  7
            var seven_test = businessLogic.GetMaxID();

            Console.WriteLine("test GetMaxID ");
            Console.WriteLine(seven_test);
            Console.WriteLine();

            //test GetOrderedUsers  8,9,10
            var eight_test = businessLogic.GetOrderedUsers();
            Console.WriteLine("test GetOrderedUsers ");

            foreach (var user in eight_test)
            {
                Console.Write(user.Name + " ");
            }
            Console.WriteLine();


            //test GetUsersPage  11
            var eleven = businessLogic.GetUsersPage(1,3);
            Console.WriteLine("test GetUsersPage ");

            foreach (var user in eight_test)
            {
                Console.Write(user.Name + " ");
                
            }
            Console.WriteLine();
        }
    }
}
//add new line sssssssssss
