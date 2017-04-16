using System.Text;

public class LingzeTool
{
    //将数字转换成Excel的A,B,AB格式 1=A,2=B,28=AB
    public static string ConvertToTitle(int n)
    {
        //1:如果小于0或者无限大，则表示不符合规则
        if (n <= 0 || n > int.MaxValue)
            return null;
        //2.小于等于26直接从字典拿
        if (n <= 26)
            return ((char)('A' + n - 1)).ToString();
        //3.其他情况
        return excelToNum(n);
    }

    private static string excelToNum(int number)
    {
        StringBuilder sb = new StringBuilder();
        int initNumberTemp = number;
        int tempMod = 0;
        while (initNumberTemp > 0)
        {
            tempMod = initNumberTemp % 26;
            initNumberTemp /= 26;
            if (tempMod > 0)
                sb.Insert(0, (char)('A' + tempMod - 1));
            else
            {
                initNumberTemp -= 1;
                sb.Insert(0, (char)('A' + 25));
            }
        }
        return sb.ToString();
    }
}