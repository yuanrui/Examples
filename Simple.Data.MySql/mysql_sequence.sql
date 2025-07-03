-- https://yuanrui.github.io/2021/04/01/MySql-create-sequence-like-oracle/
delimiter //
drop table if exists t_sequence;
create table t_sequence
(
  seq_name varchar(64) not null primary key comment '序列名称',
  start_value bigint(20) not null default 1 comment '起始值',
  increment int(11) not null default 1 comment '增量值',
  updated_at timestamp not null default CURRENT_TIMESTAMP on update CURRENT_TIMESTAMP comment '更新时间'
);
alter table t_sequence comment '序列表';

-- 启用函数创建功能
-- set global log_bin_trust_function_creators=1;

drop function if exists setval;
create function setval(vseq varchar(64), vstart bigint(20))
returns bigint(20)
begin
  insert into t_sequence(seq_name, start_value, increment, updated_at)
  values (vseq, vstart + 1, 1, CURRENT_TIMESTAMP())
  on duplicate key update start_value = vstart + increment;
  
  return vstart;
end;

drop function if exists nextval;
create function nextval(vseq varchar(64))
returns bigint(20)
begin
  update t_sequence
  set start_value = (@next := start_value) + increment
  where seq_name = vseq;
  
  return ifnull(@next, setval(vseq, 1));
end;

drop function if exists lastval;
create function lastval(vseq varchar(64))
returns bigint(20)
begin
  declare cur_val bigint(20);
  
  select start_value into cur_val 
  from t_sequence
  where seq_name = vseq;
  
  return ifnull(cur_val, setval(vseq, 1));
end;
//
delimiter ;