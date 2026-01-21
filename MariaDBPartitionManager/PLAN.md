# MariaDB Partition Manager 구현 계획

이 프로젝트는 MariaDB의 테이블 파티션을 관리하기 위한 Qt (C++) 기반 애플리케이션입니다.

## 1. 프로젝트 구조
- `MariaDBPartitionManager/`
  - `CMakeLists.txt`: 프로젝트 설정 및 Qt 라이브러리 연결
  - `main.cpp`: 애플리케이션 진입점
  - `MainWindow.h/cpp/ui`: 메인 UI (파티션 목록 표시 및 제어)
  - `DatabaseManager.h/cpp`: MariaDB 연결 및 파티션 관련 SQL 쿼리 로직

## 2. 주요 기능
- **DB 연결**: 호스트, 사용자, 비밀번호를 입력받아 MariaDB 연결
- **파티션 조회**: `information_schema.PARTITIONS` 테이블을 쿼리하여 특정 테이블의 파티션 상태(이름, 범위, 데이터 로우 수 등)를 조회
- **파티션 관리**: 
    - `ALTER TABLE ... ADD PARTITION`을 통한 파티션 추가
    - `ALTER TABLE ... DROP PARTITION`을 통한 파티션 삭제
- **시각화**: 파티션별 데이터 분포를 간단한 리스트나 차트로 표시

## 3. 기술 스택
- **Framework**: Qt 6 (Widgets, Sql)
- **Language**: C++ 17 이상
- **DB Driver**: QMYSQL (MariaDB 호환)

## 4. 향후 단계
1. `CMakeLists.txt` 및 기본 소스 코드 생성
2. `DatabaseManager` 클래스 구현 (MariaDB 특화 쿼리 포함)
3. UI 디자인 및 데이터 바인딩
