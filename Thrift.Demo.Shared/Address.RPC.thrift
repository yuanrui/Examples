namespace csharp Thrift.Demo.Shared

struct AddressDTO {
	/** 地点编号 */
    1: required i64 ADDR_ID

	/** 地点名 */
    2: required string ADDRESS

	/** 区域 */
    3: optional string AREA

	/** 经度 */
    4: required double FLONG

	/** 纬度 */
    5: required double LAT

	/** 备注 */
    6: optional string REMARK

	7: optional DateTime2 Time
	
	8: optional DateTime2 Time2	
}

struct DateTime2 {
      1: i64 Value
}

service AddressRpc {
    i64 GetNewId();
    bool Exists(1:i64 ADDR_ID);
    AddressDTO Get(1:i64 ADDR_ID);
    bool Add(1:AddressDTO dto);
    bool Add2(1:list<AddressDTO> list2);
    bool Update(1:AddressDTO dto);
    bool Save(1:AddressDTO dto);
    bool Delete(1:i64 ADDR_ID);
}

