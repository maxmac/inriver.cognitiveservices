# Microsoft Cogntivie Services for inRiver iPMC

Runs as an EntityListener on Resource entities. Reads text from image and then translates it to all available languages in the current environment.

## Requirements
### Resource
* string ResourceImageText
* LocaleString ResourceImageTextTranslated

### Server Settings
* CognitiveServicesImageOcrService_ApiKey
* CognitiveServicesTranslationService_ApiKey

Get your API keys from the Azure Portal (https://portal.azure.com) by creating the following services
* Translator Text
* Computer Vision (North Europe)

And take the given keys and put them inside the server settings above

### Image Configuration
jpg;png
visualrecognition
-type truecolor -quality 90 -colorspace rgb -density 75 "{0}" -resize 3200x3200 "{1}"

### EntityListener in iPMC
* Assembly Name: Inriver.CognitiveServices.dll
* Assembly Type: Inriver.CognitiveServices.CognitiveServicesExtensionListener
* Extension Type: EntityListener