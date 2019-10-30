using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using MVVM.Core.ViewModels;

namespace MVVM.Extension.Services
{
    public interface IViewModelProvider
    {
        List<string> ViewModels { get; }
        List<string> GetViewModels();
        ViewModelBase GetViewModelBehaviour(string viewModelName);
    }

    public abstract class ViewModelProvider : IViewModelProvider
    {
        #region Static Properties
        public static IViewModelProvider Instance { get; private set; }
        #endregion

#region Constructors
        static ViewModelProvider()
        {
            InitializeMe();
        }

        protected ViewModelProvider()
        {
            Instance = this;
        }
#endregion

        #region Abstract Methods
        public abstract List<string> ViewModels { get; }
        public abstract List<string> GetViewModels();
        public abstract ViewModelBase GetViewModelBehaviour(string viewModelName);
        #endregion

        #region Static Fields

        private static bool s_IsInitialized = false;
        protected static List<string> s_ViewModels = null;
        protected static IDictionary<string, Type> s_GlobalTypeMap;
        protected static readonly Type s_ViewModelBaseType = typeof(ViewModelBase);
        #endregion

        #region Static Methods

        private static void InitializeMe()
        {
            if (s_IsInitialized) return;
            RefreshGlobalTypeMap();
            s_IsInitialized = true;
            s_ViewModels = SGetViewModels();
        }
        
        protected static void RefreshGlobalTypeMap()
        {
            s_GlobalTypeMap = new Dictionary<string, Type>();
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                var types = asm.GetTypes().Where(e => e.IsSubclassOf(s_ViewModelBaseType) && !e.IsAbstract).Select(e => new KeyValuePair<string, Type>(e.ToString(), e));
                foreach (var t in types)
                {
                    s_GlobalTypeMap.Add(t);
                }
            }
        }

        public static Type GetViewModelType(string typeString)
        {
            InitializeMe();
            return s_GlobalTypeMap[typeString];
        }

        public static List<string> SGetViewModels()
        {
            InitializeMe();
            return s_GlobalTypeMap?.Keys.ToList();
        }

        public static bool IsViewModelTypeNameValid(string typeString)
        {
            InitializeMe();
            return !string.IsNullOrEmpty(typeString) && s_ViewModels.Contains(typeString);
        }
        #endregion
    }
}
