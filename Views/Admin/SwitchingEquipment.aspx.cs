using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

public partial class equipments : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Button2_Click(object sender, EventArgs e)
    {
        using (var con = new SqlConnection(ConfigurationManager.ConnectionStrings["qwe"].ConnectionString))
        {
            con.Open();
            var str = new SqlCommand(@"INSERT INTO tc_switching_equipment (id_model, id_nodes, id_owner, serial_number, inventory_number, mac, ip) 
                VALUES (@id_model, @id_nodes, @id_owner, @serial_number, @inventory_number, @mac, @ip)", con);
            str.Parameters.Clear();
            //str.Parameters.AddWithValue("@teacher", Session["UserAuthentication"]);
            str.Parameters.AddWithValue("@id_model", DropDownList2.SelectedValue);
            str.Parameters.AddWithValue("@id_nodes", DropDownList1.SelectedValue);
            str.Parameters.AddWithValue("@id_owner", DropDownList3.SelectedValue);
            str.Parameters.AddWithValue("@serial_number", TextBox2.Text);
            str.Parameters.AddWithValue("@inventory_number", TextBox3.Text);
            str.Parameters.AddWithValue("@mac", TextBox1.Text);
            str.Parameters.AddWithValue("@ip", TextBox8.Text);
            str.ExecuteNonQuery();
            GridView1.DataBind();
        }
    }

    protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        GridView1.PageIndex = e.NewPageIndex;
    }

    protected void GridView1_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        var ddl1 = (DropDownList)GridView1.Rows[e.RowIndex].FindControl("DropDownList1") as DropDownList;
        e.NewValues["id_nodes"] = ddl1.SelectedValue;
        var ddl2 = (DropDownList)GridView1.Rows[e.RowIndex].FindControl("DropDownList2") as DropDownList;
        e.NewValues["id_model"] = ddl2.SelectedValue;
        var ddl3 = (DropDownList)GridView1.Rows[e.RowIndex].FindControl("DropDownList3") as DropDownList;
        e.NewValues["id_owner"] = ddl3.SelectedValue;
    }
}