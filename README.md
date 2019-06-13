# SSW.TimePRO.AutoTimesheeting
A attempt to make my time sheeting easier with help of Cypress.

## Postman queries

Get token API: https://ssw.sswtimepro.com/b/admin/api-key

When running Azure Functions locally:
curl -X GET \
  'http://localhost:7071/api/CrmAppointments?tenantUrl=https://ssw.sswtimepro.com&empID=JEK&start=2019-05-25T00:00:00%2B10&end=2019-05-29T00:00:00%2B10&token=[SSW_TimePro_Token]' \
  -H 'Postman-Token: 0bd3a2c9-cad2-46db-a38c-808ad75043b6' \
  -H 'cache-control: no-cache'
