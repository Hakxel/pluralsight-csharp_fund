using System;
using System.Collections.Generic;
using System.IO;

namespace GradeBook 
{

    public delegate void GradeAddedDelegate(object sender, EventArgs args);

    public class NamedObject
    {

        public NamedObject(string name)
        {
            Name = name;
        }
        public string Name
        {
            get;
            set;
        }
    }

    public interface IBook
    {
        void AddGrade(double grade);
        Statistics GetStatistics();
        string Name {get; }
        event GradeAddedDelegate GradeAdded;

    }

    public abstract class Book : NamedObject, IBook
    {
        public Book(string name) : base(name)
        {
        }

        public abstract event GradeAddedDelegate GradeAdded;

        public abstract void AddGrade(double grade);

        public abstract Statistics GetStatistics()
        
    }

    public class DiskBook : Book
    {
        public DiskBook(string name) : base(name)
        {
        }

        public override event GradeAddedDelegate GradeAdded;

        public override void AddGrade(double grade)
        {
            var writer = File.AppendText($"{Name}.txt");
            writer.WriteLine(grade);
        }

        public override Statistics GetStatistics()
        {
            throw new NotImplementedException();
        }
    }

    public class InMemoryBook : Book
    {

        public InMemoryBook(string name) : base(name)
        {
            grades = new List<double>();
            Name = name;
        }

        public override void AddGrade(double grade) 
        {
            if(grade <= 100 && grade > 0)
            {
                grades.Add(grade);
                if(GradeAdded != null)
                {
                    GradeAdded(this, new EventArgs());
                }
            }
            else
            {
                throw new ArgumentException($"Invalid {nameof(grade)}");
            }
        }

        public override event GradeAddedDelegate GradeAdded;

        public override Statistics GetStatistics() 
        {
            var result = new Statistics();
            result.High = double.MinValue;
            result.Low = double.MaxValue;
            foreach(var grade in grades)
            {   
                // if(number > highGrade)
                // {
                //     highGrade = number;
                // }

                result.Low = Math.Min(grade, result.Low);
                result.High = Math.Max(grade, result.High);
                result.Average += grade;

            }

            result.Average /= grades.Count;

            switch(result.Average)
            {
                case var d when d >= 90:
                    result.Letter = 'A';
                    break;

                case var d when d >= 80:
                    result.Letter = 'B';
                    break;

                    
                case var d when d >= 70:
                    result.Letter = 'C';
                    break;

                    
                case var d when d >= 60:
                    result.Letter = 'D';
                    break;

                default:
                    result.Letter = 'F';
                    break;
            }

            return result;
        }

        private List<double> grades;

        // public string Name
        // {
        //     get
        //     {
        //         return name;
        //     }

        //     set
        //     {
        //         if(!String.IsNullOrEmpty(value))
        //         {
        //             name = value;
        //         }
        //     }
        // }

        // private string name;
    }
}