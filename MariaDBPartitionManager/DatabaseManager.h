#ifndef DATABASEMANAGER_H
#define DATABASEMANAGER_H

#include <QString>
#include <QSqlDatabase>
#include <QSqlQuery>
#include <QSqlError>
#include <QList>
#include <QVariant>

// 파티션 정보를 담는 구조체
struct PartitionInfo {
    QString name;           // 파티션 이름
    QString method;         // 파티션 방법 (RANGE, LIST 등)
    QString expression;     // 파티션 기준 열/식
    QString description;    // 파티션 범위/값 설명
    long long tableRows;    // 해당 파티션의 행 수
};

class DatabaseManager {
public:
    DatabaseManager();
    ~DatabaseManager();

    // DB 연결 설정
    bool connectToDatabase(const QString& host, int port, const QString& dbName, 
                          const QString& user, const QString& password);
    
    // 연결 종료
    void disconnect();

    // 특정 테이블의 파티션 목록 조회
    QList<PartitionInfo> getPartitions(const QString& tableName);

    // 파티션 추가 (Range 기준 예시)
    bool addRangePartition(const QString& tableName, const QString& partitionName, const QString& lessThanValue);

    // 파티션 삭제
    bool dropPartition(const QString& tableName, const QString& partitionName);

    // 마지막 에러 메시지 반환
    QString lastError() const { return m_lastError; }

private:
    QSqlDatabase m_db;
    QString m_lastError;
    QString m_currentDbName;
};

#endif // DATABASEMANAGER_H
