#include "MainWindow.h"
#include <QVBoxLayout>
#include <QHBoxLayout>
#include <QHeaderView>
#include <QMessageBox>
#include <QInputDialog>

MainWindow::MainWindow(QWidget *parent)
    : QMainWindow(parent) {
    setupUI();
    setWindowTitle("MariaDB Partition Manager");
    resize(800, 600);
}

MainWindow::~MainWindow() {}

void MainWindow::setupUI() {
    QWidget *centralWidget = new QWidget(this);
    setCentralWidget(centralWidget);
    QVBoxLayout *mainLayout = new QVBoxLayout(centralWidget);

    // Connection Area
    QHBoxLayout *connLayout = new QHBoxLayout();
    m_hostEdit = new QLineEdit("localhost");
    m_dbEdit = new QLineEdit("test_db");
    m_userEdit = new QLineEdit("root");
    m_passwordEdit = new QLineEdit();
    m_passwordEdit->setEchoMode(QLineEdit::Password);
    m_connectBtn = new QPushButton("연결");

    connLayout->addWidget(new QLabel("Host:"));
    connLayout->addWidget(m_hostEdit);
    connLayout->addWidget(new QLabel("DB:"));
    connLayout->addWidget(m_dbEdit);
    connLayout->addWidget(new QLabel("User:"));
    connLayout->addWidget(m_userEdit);
    connLayout->addWidget(new QLabel("PW:"));
    connLayout->addWidget(m_passwordEdit);
    connLayout->addWidget(m_connectBtn);
    mainLayout->addLayout(connLayout);

    // Table Select Area
    QHBoxLayout *tableLayout = new QHBoxLayout();
    m_tableEdit = new QLineEdit("your_table_name");
    m_refreshBtn = new QPushButton("파티션 조회");
    tableLayout->addWidget(new QLabel("Table:"));
    tableLayout->addWidget(m_tableEdit);
    tableLayout->addWidget(m_refreshBtn);
    mainLayout->addLayout(tableLayout);

    // Partition Table
    m_partitionTable = new QTableWidget(0, 5);
    m_partitionTable->setHorizontalHeaderLabels({"파티션명", "방법", "표현식", "설명(범위)", "행 수"});
    m_partitionTable->horizontalHeader()->setSectionResizeMode(QHeaderView::Stretch);
    mainLayout->addWidget(m_partitionTable);

    // Actions
    QHBoxLayout *actionLayout = new QHBoxLayout();
    m_addBtn = new QPushButton("파티션 추가");
    m_dropBtn = new QPushButton("선택 파티션 삭제");
    actionLayout->addWidget(m_addBtn);
    actionLayout->addWidget(m_dropBtn);
    mainLayout->addLayout(actionLayout);

    // Signals
    connect(m_connectBtn, &QPushButton::clicked, this, &MainWindow::handleConnect);
    connect(m_refreshBtn, &QPushButton::clicked, this, &MainWindow::refreshPartitionList);
    connect(m_addBtn, &QPushButton::clicked, this, &MainWindow::handleAddPartition);
    connect(m_dropBtn, &QPushButton::clicked, this, &MainWindow::handleDropPartition);
}

void MainWindow::handleConnect() {
    bool ok = m_dbMgr.connectToDatabase(m_hostEdit->text(), 3306, m_dbEdit->text(), 
                                      m_userEdit->text(), m_passwordEdit->text());
    if (ok) {
        QMessageBox::information(this, "연결 성공", "MariaDB에 성공적으로 연결되었습니다.");
    } else {
        QMessageBox::critical(this, "연결 실패", m_dbMgr.lastError());
    }
}

void MainWindow::refreshPartitionList() {
    QString tableName = m_tableEdit->text();
    auto partitions = m_dbMgr.getPartitions(tableName);
    
    m_partitionTable->setRowCount(0);
    for (const auto& p : partitions) {
        int row = m_partitionTable->rowCount();
        m_partitionTable->insertRow(row);
        m_partitionTable->setItem(row, 0, new QTableWidgetItem(p.name));
        m_partitionTable->setItem(row, 1, new QTableWidgetItem(p.method));
        m_partitionTable->setItem(row, 2, new QTableWidgetItem(p.expression));
        m_partitionTable->setItem(row, 3, new QTableWidgetItem(p.description));
        m_partitionTable->setItem(row, 4, new QTableWidgetItem(QString::number(p.tableRows)));
    }
}

void MainWindow::handleAddPartition() {
    bool ok;
    QString pName = QInputDialog::getText(this, "파티션 추가", "파티션 이름:", QLineEdit::Normal, "", &ok);
    if (!ok || pName.isEmpty()) return;
    
    QString pValue = QInputDialog::getText(this, "파티션 추가", "LESS THAN 값 (예: 20240101):", QLineEdit::Normal, "", &ok);
    if (!ok || pValue.isEmpty()) return;

    if (m_dbMgr.addRangePartition(m_tableEdit->text(), pName, pValue)) {
        QMessageBox::information(this, "성공", "파티션이 추가되었습니다.");
        refreshPartitionList();
    } else {
        QMessageBox::critical(this, "오류", m_dbMgr.lastError());
    }
}

void MainWindow::handleDropPartition() {
    int row = m_partitionTable->currentRow();
    if (row < 0) {
        QMessageBox::warning(this, "경고", "삭제할 파티션을 테이블에서 선택해주세요.");
        return;
    }
    
    QString pName = m_partitionTable->item(row, 0)->text();
    auto reply = QMessageBox::question(this, "확인", QString("파티션 [%1]을(를) 삭제하시겠습니까?\n데이터도 함께 삭제됩니다!").arg(pName));
    
    if (reply == QMessageBox::Yes) {
        if (m_dbMgr.dropPartition(m_tableEdit->text(), pName)) {
            QMessageBox::information(this, "성공", "파티션이 삭제되었습니다.");
            refreshPartitionList();
        } else {
            QMessageBox::critical(this, "오류", m_dbMgr.lastError());
        }
    }
}
