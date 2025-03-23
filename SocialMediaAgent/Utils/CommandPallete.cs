namespace SocialMediaAgent.Utils
{
    public class CommandPallete()
    {
        //dict<command, actionmethod or di>
        public static Dictionary<string, Action> cmd = new(){
            {"/generate-post", GeneratePost}
        };

        private static void GeneratePost()
        {
            throw new NotImplementedException();
        }
    }
}