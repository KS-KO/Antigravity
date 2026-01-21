#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <QTableWidget>
#include <QPushButton>
#include <QLineEdit>
#include <QLabel>
#include "DatabaseManager.h"

class MainWindow : public QMainWindow {
    Q_OBJECT

public:
    MainWindow(QWidget *parent = nullptr);
    ~MainWindow();

private slots:
    void handleConnect();
    void refreshPartitionList();
    void handleAddPartition();
    void handleDropPartition();

private:
    void setupUI();
    
    DatabaseManager m_dbMgr;
    
    // UI Elements
    QLineEdit *m_hostEdit;
    QLineEdit *m_dbEdit;
    QLineEdit *m_userEdit;
    QLineEdit *m_passwordEdit;
    QLineEdit *m_tableEdit;
    
    QTableWidget *m_partitionTable;
    
    QPushButton *m_connectBtn;
    QPushButton *m_refreshBtn;
    QPushButton *m_addBtn;
    QPushButton *m_dropBtn;
};

#endif // MAINWINDOW_H
