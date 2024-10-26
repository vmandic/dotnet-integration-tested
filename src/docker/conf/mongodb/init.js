const conn = new Mongo();
const dbName = "dotnet_integration_tested";

// Use the selected database
const db = conn.getDB(dbName);

// seo_scores
db.seo_scores.createIndex({
  user_id: 1,
});
