#include "DatabaseManager.h"
#include <QDebug>
#include <QSqlRecord>

DatabaseManager::DatabaseManager() {
}

DatabaseManager::~DatabaseManager() {
    disconnect();
}

bool DatabaseManager::connectToDatabase(const QString& host, int port, const QString& dbName, 
                                      const QString& user, const QString& password) {
    // MariaDB/MySQL 드라이버 사용
    m_db = QSqlDatabase::addDatabase("QMYSQL");
    m_db.setHostName(host);
    m_db.setPort(port);
    m_db.setDatabaseName(dbName);
    m_db.setUserName(user);
    m_db.setPassword(password);

    if (!m_db.open()) {
        m_lastError = m_db.lastError().text();
        return false;
    }

    m_currentDbName = dbName;
    return true;
}

void DatabaseManager::disconnect() {
    if (m_db.isOpen()) {
        m_db.close();
    }
}

QList<PartitionInfo> DatabaseManager::getPartitions(const QString& tableName) {
    QList<PartitionInfo> partitions;
    
    // information_schema 에서 파티션 정보 조회
    QSqlQuery query(m_db);
    query.prepare("SELECT PARTITION_NAME, PARTITION_METHOD, PARTITION_EXPRESSION, "
                  "PARTITION_DESCRIPTION, TABLE_ROWS "
                  "FROM information_schema.PARTITIONS "
                  "WHERE TABLE_SCHEMA = :dbName AND TABLE_NAME = :tableName "
                  "AND PARTITION_NAME IS NOT NULL "
                  "ORDER BY PARTITION_ORDINAL_POSITION");
    
    query.bindValue(":dbName", m_currentDbName);
    query.bindValue(":tableName", tableName);

    if (!query.exec()) {
        m_lastError = query.lastError().text();
        qDebug() << "Query failed:" << m_lastError;
        return partitions;
    }

    while (query.next()) {
        PartitionInfo info;
        info.name = query.value("PARTITION_NAME").toString();
        info.method = query.value("PARTITION_METHOD").toString();
        info.expression = query.value("PARTITION_EXPRESSION").toString();
        info.description = query.value("PARTITION_DESCRIPTION").toString();
        info.tableRows = query.value("TABLE_ROWS").toLongLong();
        partitions.append(info);
    }

    return partitions;
}

bool DatabaseManager::addRangePartition(const QString& tableName, const QString& partitionName, const QString& lessThanValue) {
    QSqlQuery query(m_db);
    // 예: ALTER TABLE table_name ADD PARTITION (PARTITION p_name VALUES LESS THAN (value))
    QString sql = QString("ALTER TABLE %1 ADD PARTITION (PARTITION %2 VALUES LESS THAN (%3))")
                    .arg(tableName)
                    .arg(partitionName)
                    .arg(lessThanValue);
    
    if (!query.exec(sql)) {
        m_lastError = query.lastError().text();
        return false;
    }
    return true;
}

bool DatabaseManager::dropPartition(const QString& tableName, const QString& partitionName) {
    QSqlQuery query(m_db);
    // 예: ALTER TABLE table_name DROP PARTITION p_name
    QString sql = QString("ALTER TABLE %1 DROP PARTITION %2")
                    .arg(tableName)
                    .arg(partitionName);
    
    if (!query.exec(sql)) {
        m_lastError = query.lastError().text();
        return false;
    }
    return true;
}
