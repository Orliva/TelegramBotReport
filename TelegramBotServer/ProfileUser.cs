using System;

namespace TelegramBotServer.Users
{
    [Serializable]
    public class ProfileUser : ICloneable
    {
        public string FamilyName { get; set; } //Фамилия
        public string FirstName { get; set; } //Имя
        public string SecondName { get; set; } //Отчество
        public string UserPost { get; set; } //Должность
        public bool IsAdminAcc { get; private set; }//Является ли юзер админом

        public ProfileUser()
        {
            FamilyName = "Non";
            FirstName = "Non";
            SecondName = "Non";
            UserPost = "Non";
            IsAdminAcc = false;
        }

        public ProfileUser(string familyName, string firstName, string secondName, string userPost, bool isAdminAcc)
        {
            FamilyName = familyName;
            FirstName = firstName;
            SecondName = secondName;
            UserPost = userPost;
            IsAdminAcc = isAdminAcc;
        }

        public object Clone()
        {
            return new ProfileUser()
            {
                FamilyName = this.FamilyName.Clone() as string,
                FirstName = this.FirstName.Clone() as string,
                SecondName = this.SecondName.Clone() as string,
                UserPost = this.UserPost.Clone() as string,
                IsAdminAcc = IsAdminAcc
            };
        }

        public bool TryChangeAdminRights(TelegramUser adminAccount, TelegramUser destAccount, bool isAdmin)
        {
            if (adminAccount != null && adminAccount.Profile != null && adminAccount.Profile.IsAdminAcc == true)
            {
                if (destAccount != null && destAccount.Profile != null)
                {
                    destAccount.Profile.IsAdminAcc = isAdmin;
                    return true;
                }
            }
            return false;
        }

        public string GetFullName() => FamilyName + " " + FirstName + SecondName;
    }
}
