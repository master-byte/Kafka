 CREATE TABLE IF NOT EXISTS alfn.primes
(
    id  UInt32 NOT NULL,    
    number  UInt32 NOT NULL,
    nick_name  String NOT NULL, 
    date_number DateTime(),
    date_queue DateTime()
)
ENGINE = MergeTree()
PRIMARY KEY (id);