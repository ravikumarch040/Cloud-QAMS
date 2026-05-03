targetScope = 'resourceGroup'

@description('Deployment environment name.')
param environmentName string

@description('Azure region for QAMS resources.')
param location string = resourceGroup().location

@description('Short application name used in resource names.')
param applicationName string = 'qams'

var namePrefix = '${applicationName}-${environmentName}'

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: '${namePrefix}-kv'
  location: location
  properties: {
    tenantId: tenant().tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    enableRbacAuthorization: true
    enabledForDeployment: false
    enabledForDiskEncryption: false
    enabledForTemplateDeployment: false
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: '${namePrefix}-appi'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

output keyVaultName string = keyVault.name
output applicationInsightsName string = appInsights.name
