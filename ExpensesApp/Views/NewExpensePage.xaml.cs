using System;
using System.Collections.Generic;
using ExpensesApp.ViewModels;
using Xamarin.Forms;

namespace ExpensesApp.Views
{
    public partial class NewExpensePage : ContentPage
    {
        NewExpenseVM ViewModel;

        public NewExpensePage()
        {
            InitializeComponent();

            ViewModel = Resources["vm"] as NewExpenseVM;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            ViewModel.GetExpenseStatus();

            int count = 0;
            var section = new TableSection("Statuses");
            foreach (var es in ViewModel.ExpenseStatuses)
            {
                var cell = new SwitchCell { Text = es.Name };

                Binding binding = new Binding();
                binding.Source = ViewModel.ExpenseStatuses[count];
                binding.Path = "Status";
                binding.Mode = BindingMode.TwoWay;
                cell.SetBinding(SwitchCell.OnProperty, binding);

                section.Add(cell);

                count++;
            }
            expenseTableView.Root.Add(section);
        }
    }
}
