﻿using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using ExpensesApp.Interfaces;
using ExpensesApp.Models;
using ExpensesApp.Resources;
using PCLStorage;
using Xamarin.Forms;

namespace ExpensesApp.ViewModels
{
    public class CategoriesVM
    {
        public ObservableCollection<string> Categories
        {
            get;
            set;
        }

        public ObservableCollection<CategoryExpenses> CategoryExpensesCollection
        {
            get;
            set;
        }

        public Command ExportCommand
        {
            get;
            set;
        }

        public CategoriesVM()
        {
            ExportCommand = new Command(ShareReport);
            Categories = new ObservableCollection<string>();
            CategoryExpensesCollection = new ObservableCollection<CategoryExpenses>();

            GetCategories();
            GetExpensesPerCategory();
        }

        private void GetCategories()
        {
            Categories.Clear();
            Categories.Add(AppResources.housingCategory);
            Categories.Add(AppResources.debtCategory);
            Categories.Add(AppResources.healthCategory);
            Categories.Add(AppResources.foodCategory);
            Categories.Add(AppResources.personalCategory);
            Categories.Add(AppResources.travelCategory);
            Categories.Add(AppResources.otherCategory);
        }

        public void GetExpensesPerCategory()
        {
            CategoryExpensesCollection.Clear();
            float totalExpensesAmount = Expense.TotalExpensesAmount();
            foreach (string c in Categories)
            {
                var expenses = Expense.GetExpenses(c);
                float expensesAmountInCategory = expenses.Sum(e => e.Amount);

                CategoryExpenses ce = new CategoryExpenses()
                {
                    Category = c,
                    ExpensesPercentage = expensesAmountInCategory / totalExpensesAmount
                };

                CategoryExpensesCollection.Add(ce);
            }
        }

        public async void ShareReport()
        {
            IFileSystem fileSystem = FileSystem.Current;
            IFolder rootFolder = fileSystem.LocalStorage;
            IFolder reportsFolder = await rootFolder.CreateFolderAsync("reports", CreationCollisionOption.OpenIfExists);

            var txtFile = await reportsFolder.CreateFileAsync("report.txt", CreationCollisionOption.ReplaceExisting);

            using (StreamWriter sw = new StreamWriter(txtFile.Path))
            {
                foreach (var ce in CategoryExpensesCollection)
                {
                    sw.WriteLine($"{ce.Category} - {ce.ExpensesPercentage:p}");
                }
            }

            IShare shareDependency = DependencyService.Get<IShare>();
            await shareDependency.Show("Expense Report", "Report", txtFile.Path);
        }

        public class CategoryExpenses
        {
            public string Category
            {
                get;
                set;
            }

            public float ExpensesPercentage
            {
                get;
                set;
            }
        }
    }
}
