# sql-table-to-json
Convert SQL DataTable SQL Datatable to JSON format. 
Allowed converstion in this version 1.0.1:
A{}
B[]
A{B{}}
A{B[]}

Example: 

SELECT 
ta.colA0 AS colA0, -- Primary Key
ta.colA1 AS colA1,
tb.colB0 AS B.colB0,
tb.colB1 AS B.colB1,
tc.colC0 AS B.C[colC0],
tc.colC1 AS B.C[colC1]
td.colD0 AS D[colD0],
td.colD1 AS D[colD1]

FROM TableA ta --Parent Table
LEFT JOIN TableB tb on ta.colA0 = tb.colB0
LEFT JOIN TableC tc on tb.colB0 = tc.colC0
LEFT JOIN TableD td on tc.colC0 = td.colD0

+-----------------------------------------------------------------------------+
| colA0  colA1  B.colB0  B.colB1  B.C[colC0]  B.C[colC1]  D[colD0] D[colD1]   |
+-----------------------------------------------------------------------------+
| 1        ARow1  BRow11  BRow12  CRow11      CRow11A      DRow11  DRow11A    |
| 1        ARow1  BRow11  BRow12  CRow12      CRow12A      DRow12  DRow12A    |
+-----------------------------------------------------------------------------+

Result:
[
  {
    "colA0": 1,
    "colA1": "ARow1",
    "B": {
      "colB0": "BRow11",
      "colB1": "BRow12",
      "C": [
        {"colC0": "CRow11","colC1": "CRow11A"},
        {"colC0": "CRow12","colC1": "CRow12A"}
      ]
    },
    "D": [
        {"colD0": "DRow11","colD1": "DRow11A"}
    ]
  }
]
