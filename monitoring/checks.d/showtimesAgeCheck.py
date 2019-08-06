from checks import AgentCheck
import requests

class ShowtimesAgeCheck(AgentCheck):
    def check(self, instance):
        response = requests.get('https://api.kinodnes.cz/api/kino/get/showtimesage')

        response_content = response.content.decode()
        age = int(response_content)
        
if __name__ == '__main__':
    check, instances = ShowtimesAgeCheck.from_yaml('/etc/dd-agent/conf.d/showtimesAgeCheck.yaml')
    for instance in instances:
        print "\nRunning the check"
        check.check(instance)
        print "\nCheck finished"
