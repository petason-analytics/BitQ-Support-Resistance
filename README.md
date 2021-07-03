#README - log
6/30/2021: Try to using v2 calculate peak trough by line chart (view exactly peak trough when changing to line chart instead of candlestick chart) without threshold.
7/1/2021: Calculate Resistance and Support separately.
7/3/2021: Try to calculate line that can be RS, SP with mix data peak, trough. But it creates too many lines.
7/3/2021: Add condition that needs to be a RS, SP at first 2 point to consider a valid lines --> OK.



#Thinking:
- Một điểm đỉnh đáy cũng có thể coi là RS, SP cho các nến sau (tuy nhiên cần 2 điểm để chắc chắn hơn, và các đỉnh đáy đó thể hiện 1 xu hướng đảo ngược mạnh)
- Bỏ qua các khái niệm RS, SP cho các đường vẽ vì nó có thể thay đổi tùy theo nến hiện tại.


#TODO:
- Cần xác định những đỉnh, đáy có xu thế đảo ngược mạnh để khử nhiều nhiều đường chưa chuẩn.
- Cần xác định sức mạnh của các đường này: 
    - Có bao nhiều đỉnh đáy đã xuyên qua ?
    - Có bao nhiêu nến râu dài bị cản bởi các đường này ?
    - Xu thế đảo chiều ở các đường khi chạm tới đường này có mạnh ko ở quá khứ ?
- Các đường line trên sẽ được kéo dài 300 nến trong phạm vị của nó để nhìn thấy hiệu quả apply (Hiện tại đang apply mỗi điểm đầu và điểm cuối, chưa thấy trong range nào có thể áp dụng ?? )
