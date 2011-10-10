namespace PowerSDR
{
    public class ATUDLLTest
    {
        public static void test()
        {
#if(!NO_NEW_ATU)
            ATUClass obj = new ATUClass();
            obj.DllExists();
#endif
        }
    }
}