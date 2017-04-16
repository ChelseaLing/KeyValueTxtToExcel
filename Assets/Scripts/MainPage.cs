using UnityEngine;
using System.Windows.Forms;

public enum TipsType
{
    ERROR,
    TIPS
}

public class MainPage : MonoBehaviour
{
    public static MainPage Ins;
    //导出
    public GameObject openDirDialogBtn;
    public GameObject exportDialogBtn;
    public UILabel dirLab;
    //生成
    public GameObject openFileDialogBtn;
    public GameObject createDialogBtn;
    public UILabel fileLab;
    //提示
    public UILabel tipsLabel;

    private void Awake()
    {
        Ins = this;
        UnityEngine.Screen.SetResolution(800, 440, false);
        UIEventListener.Get(openDirDialogBtn).onClick = openDirDialog;
        UIEventListener.Get(exportDialogBtn).onClick = exportExcelDialog;
        UIEventListener.Get(openFileDialogBtn).onClick = openFileDialog;
        UIEventListener.Get(createDialogBtn).onClick = createTxtDialog;
        if (!Config.initConfig())
        {
            setTips("读取配置文件失败，请检查配置文件是否存在");
            return;
        }
        if (string.IsNullOrEmpty(Config.defalutDirPath))
        {
            setTips("读取目录失败，请重新选择路径");
            return;
        }
        if (Config.multilingualKey.Count <= 0)
        {
            setTips("配置文件内多语言不存在，请检查配置文件");
            return;
        }
        setTips("");
        dirLab.text = Config.defalutDirPath;
    }

    public void setTips(string tip, TipsType type = TipsType.ERROR)
    {
        switch (type)
        {
            case TipsType.ERROR:
                tipsLabel.text = "[ff0000]" + tip + "[-]";
                break;
            case TipsType.TIPS:
                tipsLabel.text = "[099432]" + tip + "[-]";
                break;
            default:
                break;
        }
    }

    private void openDirDialog(GameObject go)
    {
        FolderBrowserDialog dilog = new FolderBrowserDialog();
        dilog.Description = "请选择文件夹";
        if (dilog.ShowDialog() == DialogResult.OK || dilog.ShowDialog() == DialogResult.Yes)
        {
            Config.defalutDirPath = dilog.SelectedPath;
            dirLab.text = Config.defalutDirPath;
            Filehandle.ChangeConfig("defalutDirPath", dilog.SelectedPath);
            if (!Config.initConfig())
            {
                setTips("读取配置文件失败，请检查配置文件是否存在");
                return;
            }
            setTips("");
        }
        else
        {
            dilog.Dispose();
        }
    }

    private void exportExcelDialog(GameObject go)
    {
        WriteExcel.InitExportExcel();
        setTips("生成Excel成功！", TipsType.TIPS);
    }

    private void openFileDialog(GameObject go)
    {
        OpenFileDialog ofd = new OpenFileDialog();
        ofd.InitialDirectory = "C:\\";
        ofd.Filter = "Excel-2007(*.xlsx)|*.xlsx|Excel-2003(*.xls)|*.xls|所有文件(*.*)|*.*";
        if (ofd.ShowDialog() == DialogResult.OK)
        {
            fileLab.text = ofd.FileName;
        }
    }

    private void createTxtDialog(GameObject go)
    {
        FolderBrowserDialog dilog = new FolderBrowserDialog();
        dilog.Description = "请选择生成语言包的文件夹";
        if (dilog.ShowDialog() == DialogResult.OK || dilog.ShowDialog() == DialogResult.Yes)
        {
            AnalysisExcel.parseExcel(fileLab.text, dilog.SelectedPath);
            setTips("Excel转换成语言包成功！", TipsType.TIPS);
        }
        else
        {
            dilog.Dispose();
        }
    }
}