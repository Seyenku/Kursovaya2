using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

public partial class models : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            chkColumn1.Checked = true;
        }
        groupTable();
    }

    protected void ListView1_ItemDataBound(object sender, ListViewItemEventArgs e)
    {
        groupTable();
        var tm = ports;
        var tr = new SqlDataSource();
        tr.ConnectionString = tm.ConnectionString;
        tr.SelectCommand = tm.SelectCommand;
        tr.SelectParameters.Add(new Parameter("model_id"));
        tr.SelectParameters["model_id"].DefaultValue = ListView1.DataKeys[e.Item.DataItemIndex]["id"].ToString();
        ListView portes = e.Item.FindControl("ListView2") as ListView;
        portes.DataSource = tr;
        portes.DataBind();
    }


    public class SqlCheckBox
    {
        private string firstSelectDistinct;
        private string groupBy;
        private string secondSelectDistinct;
        private string innerJoin;

        public SqlCheckBox(string firstSelectDistinct, string groupBy, string secondSelectDistinct, string innerJoin)
        {
            this.firstSelectDistinct = firstSelectDistinct;
            this.groupBy = groupBy;
            this.secondSelectDistinct = secondSelectDistinct;
            this.innerJoin = innerJoin;
        }

        public String getFirstSelectDistinct()
        {
            return firstSelectDistinct;
        }
        public String getGroupBy()
        {
            return groupBy;
        }
        public String getSecondSelectDistinct()
        {
            return secondSelectDistinct;
        }
        public String getInnerJoin()
        {
            return innerJoin;
        }
    }


    public void groupTable()
    {
        SqlCheckBox CheckBox1 = new SqlCheckBox("cre.name AS creator", "cre.name", "tt.creator", "tt.creator = cre.name");
        SqlCheckBox CheckBox2 = new SqlCheckBox("m.name AS name", "m.name", "tt.name", "tt.name = m.name");
        SqlCheckBox CheckBox3 = new SqlCheckBox("teq.name AS typee", "teq.name", "tt.typee", "tt.typee = teq.name");
        SqlCheckBox CheckBox4 = new SqlCheckBox("ins.name AS inst", "ins.name", "tt.inst", "tt.inst = ins.name");
        SqlCheckBox CheckBox5 = new SqlCheckBox("m.managed AS managed", "m.managed", "tt.managed", "tt.managed = m.managed");
        SqlCheckBox CheckBox6 = new SqlCheckBox("m.console AS console", "m.console", "tt.console", "tt.console = m.console");
        SqlCheckBox CheckBox7 = new SqlCheckBox("m.poe AS poe", "m.poe", "tt.poe", "tt.poe = m.poe");

        CheckBox[] checkBoxes = { chkColumn1, chkColumn2, chkColumn3, chkColumn4, chkColumn5, chkColumn6, chkColumn7 };
        SqlCheckBox[] sqlCheckBox = { CheckBox1, CheckBox2, CheckBox3, CheckBox4, CheckBox5, CheckBox6, CheckBox7 };

        List<string> firstSelectDistinct = new List<string>();
        List<string> groupBy = new List<string>();
        List<string> secondSelectDistinct = new List<string>();
        List<string> innerJoin = new List<string>();

        for (int i = 0; i < checkBoxes.Length; i++)
        {
            if (checkBoxes[i].Checked)
            {
                firstSelectDistinct.Add(sqlCheckBox[i].getFirstSelectDistinct());
                groupBy.Add(sqlCheckBox[i].getGroupBy());
                secondSelectDistinct.Add(sqlCheckBox[i].getSecondSelectDistinct());
                innerJoin.Add(sqlCheckBox[i].getInnerJoin());
            }
            else
            {
                secondSelectDistinct.Add(sqlCheckBox[i].getFirstSelectDistinct());
            }
        }
        secondSelectDistinct = secondSelectDistinct.OrderBy(item => item.StartsWith("tt.") ? 0 : (item.Contains("AS") ? 1 : 2)).ToList();
        List<string> ttVariables = secondSelectDistinct.Where(item => item.StartsWith("tt.")).ToList();

        string selectQuery = "WITH tt AS (SELECT DISTINCT " + string.Join(", ", firstSelectDistinct) +
                              ", m.id FROM tc_model_equipment AS m " +
                              "INNER JOIN tc_equipment_type AS teq ON teq.id = m.id_type " +
                              "INNER JOIN tc_model_type_installation AS ins ON ins.id = m.type_install " +
                              "INNER JOIN tc_equipment_manufacturer AS cre ON cre.id = m.id_manufacturer " +
                              "GROUP BY " + string.Join(", ", groupBy) + ", m.id) " +
                              "SELECT DISTINCT " + string.Join(", ", secondSelectDistinct) +
                              ", m.id FROM tc_model_equipment AS m " +
                              "INNER JOIN tc_equipment_type AS teq ON teq.id = m.id_type " +
                              "INNER JOIN tc_model_type_installation AS ins ON ins.id = m.type_install " +
                              "INNER JOIN tc_equipment_manufacturer AS cre ON cre.id = m.id_manufacturer " +
                              "INNER JOIN tt ON " + string.Join(" AND ", innerJoin) +
                              " LEFT JOIN tc_model_port AS p ON p.id_model = m.id " +
                              "LEFT JOIN tc_port_type AS t ON t.id = p.id_port " +
                              "LEFT JOIN tc_port_connector_type AS con ON con.id = t.id_type " +
                              "LEFT JOIN tc_port_speed AS sp ON sp.id = t.id_speed " +
                              "WHERE (@manufacturer = '0' OR cre.name = @manufacturer) " +
                              "AND (m.name LIKE CONCAT('%', LTRIM(@namemodel), '%')) " +
                              "AND (@equipmentType = '0' OR teq.name = @equipmentType) " +
                              "AND (@installationType = '0' OR ins.name = @installationType) " +
                              "AND (@managedType = '-1' OR m.managed = @managedType) " +
                              "AND (@consolePort = '-1' OR m.console = @consolePort) " +
                              "AND (@poeSupport = '-1' OR m.poe = @poeSupport) " +
                              "AND (@port = '' OR CONCAT(p.quantity, ' x ', con.name, ' ', sp.name) LIKE CONCAT('%', @port, '%'))" +
                              "ORDER BY " + string.Join(", ", ttVariables) + ", m.id";

        modeles.SelectCommand = selectQuery;
        modeles.DataBind();
    }

    protected void GroupCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        groupTable();
    }
}