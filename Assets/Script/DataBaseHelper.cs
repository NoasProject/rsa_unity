using UnityEngine;
using System.Collections;

public class DataBaseHelper : MonoBehaviour {

private string TableName = "user";
private string db_files = "config.db";

/*
	テーブルの定義
	id -> auto int
	prime -> int
	milli_seccond -> int

	sql文
	create table user(id int auto_increment, prime BIGINT UNSIGNED, milli_seccond FLOAT UNSIGNED);
*/

	public void db_insert(int num, float milli_seccond) {
		SqliteDatabase sqlDB = new SqliteDatabase(db_files);
		string query = "insert into " + TableName + "(prime, milli_seccond) values("+num+", "+milli_seccond+")";
    sqlDB.ExecuteNonQuery(query);
	}

	public int db_total_num() {
		SqliteDatabase sqlDB = new SqliteDatabase(db_files);
		string query = "select count(*) from "+TableName;
		return int.Parse(query);
	}

	public int db_get_index_prime(int index) {
		SqliteDatabase sqlDB = new SqliteDatabase(db_files);
		string query = "select * from "+ TableName + " where prime = "+ index;
		return int.Parse(query);
	}

	public void db_delete_table_data() {
		SqliteDatabase sqlDB = new SqliteDatabase(db_files);
		string query = "delete from "+TableName;
	}
}
