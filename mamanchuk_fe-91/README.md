PREFIX TREE IMPLEMENTATION
==========================
By Mamanchuk Nicolas FE-91
--------------------------
Task Num.14
-----------
DEADlines:
----------

1. **25.10**
    - CREATE, INSERT, CONTAINS

2. **25.11**
    - SEARCH and most of its args
    
3. **25.12** or till the very end
    - PRINT_TREE and full UX

Prefix Tree V3.1A
----------------
Current update: list existing trees with the corresponding command.

Available concept of a command line.<br />
Basic system operations. pTree operations, listed below.<br />
Advanced logics that is being designed to work with user.

Operations with pTree sets available: <br />
    - `CREATE [RANGE] set_name_1 [set_name_2 ...];                           ` <br />
    - `INSERT [RANGE] set_name_1 [set_name_2 ...] "word_1" ["word_2" ...];   ` <br />
    - `SEARCH [RANGE] set_name_1 [set_name_2 ...] [WHERE query] [ASC | DESC];` <br />
    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`query:= WHERE BETWEEN "from","to" | WHERE MATCH "word"         ` <br />
    - `CONTAINS [RANGE] set_name_1 [set_name_2 ...] "word_1" ["word_2" ...]; ` <br />
    - `PRINT_TREE [RANGE] set_name_1 [set_name_2 ...];                       ` <br />
    - `LIST_EXISTING_TREES;`

System operations: <br />
    - `MENU; ` <br />
    - `CLEAR;` <br />
    - `EXIT; `

Quick notes: <br />
    Commands must include ';' at some point. <br /> 
    All parameter words must be embraced with "quotes", everything else must not. <br />
    For command titles and key words letter case is ignored.