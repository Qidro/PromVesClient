using PromVesClient.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PromVesClient.Service
{
    public class CurrentUserService
    {
        public User? CurrentUser { get; private set; }

        public bool IsAuthorized => CurrentUser != null;

        private int GraphsCount { get; set; }

        public bool UseCamera { get; set; }

        public void Login(User user, int graphsCount, bool useCamera)
        {
            CurrentUser = user;
            GraphsCount = graphsCount;
            UseCamera = useCamera;
        }
        //получение числа графиков
        public int GetGraphsCount()
        { 
            return GraphsCount;
        }
        public void Logout()
        {
            CurrentUser = null;
        }
    }
}
