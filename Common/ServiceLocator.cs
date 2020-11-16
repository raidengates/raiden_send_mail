using System;

namespace raiden_mail_reader.Common
{
    public abstract class ServiceLocator<TContract, TSelf> where TSelf : ServiceLocator<TContract, TSelf>, new()
    {
        private static Lazy<TContract> _instance = new Lazy<TContract>(InitInstance, true);
        private static TContract _testableInstance;
        private static bool _useTestable;
        protected static Func<TContract> Factory { get; set; }

        private static TContract InitInstance()
        {
            if (Factory == null)
            {
                var controllerInstance = new TSelf();
                Factory = controllerInstance.GetFactory();
            }

            return Factory();
        }
        public static TContract Instance
        {
            get
            {
                if (_useTestable)
                {
                    return _testableInstance;
                }

                return _instance.Value;
            }
        }
        public static void SetTestableInstance(TContract instance)
        {
            _testableInstance = instance;
            _useTestable = true;
        }
        public static void ClearInstance()
        {
            _useTestable = false;
            _testableInstance = default(TContract);
            _instance = new Lazy<TContract>(InitInstance, true);
        }
        protected abstract Func<TContract> GetFactory();
    }
}
