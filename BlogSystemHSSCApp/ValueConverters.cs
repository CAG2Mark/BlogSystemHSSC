using BlogSystemHSSC.Blog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace BlogSystemHSSC
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visible = (bool)value;

            if (parameter != null)
                if (parameter.ToString() == "1") visible = !visible;

            // return visibility hidden instead of collapsed
            bool hidden = false;

            if (parameter != null)
                if (parameter.ToString() == "2") hidden = true;

            return visible ? Visibility.Visible : ( hidden ? Visibility.Hidden : Visibility.Collapsed);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visible = (Visibility)value == Visibility.Visible;

            if (parameter != null)
                if ((int)parameter == 1) visible = !visible;

            return visible;
        }
    }

    public class SearchBlogPostValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            IEnumerable<BlogPost> blogPosts = (IEnumerable<BlogPost>)values[0];
            string searchTerm = (string)values[1];
            searchTerm = searchTerm.ToLower();

            if (string.IsNullOrWhiteSpace(searchTerm)) return blogPosts;

            return blogPosts.Where(x =>
                x.Title.ToLower().Contains(searchTerm) || x.Author.ToLower().Contains(searchTerm)
                );
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
